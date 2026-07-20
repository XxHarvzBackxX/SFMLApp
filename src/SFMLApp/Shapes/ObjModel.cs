using System.Globalization;
using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Utility;

namespace SFMLApp.Shapes;

/// <summary>A shape loaded from the vertex and face records of a Wavefront OBJ file.</summary>
public sealed class ObjModel : Shape3D
{
    public ObjModel(
        string path,
        float posX = 0f,
        float posY = 0f,
        float posZ = 0f,
        float rotX = 0f,
        float rotY = 0f,
        float rotZ = 0f,
        float scale = 1f,
        Color? baseShapeColor = null)
    {
        Position = new Vector3f(posX, posY, posZ);
        Rotation = new Vector3f(rotX, rotY, rotZ);
        Scale = scale;
        BaseShapeColor = baseShapeColor ?? Color.White;

        Load(path);
    }

    private void Load(string path)
    {
        List<float[]> vertices = [];
        List<int[]> faces = [];
        int lineNumber = 0;

        foreach (string line in File.ReadLines(path))
        {
            lineNumber++;
            string[] parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0 || parts[0].StartsWith('#'))
                continue;

            if (parts[0] == "v")
            {
                if (parts.Length < 4)
                    throw new FormatException($"{path}({lineNumber}): vertex requires x, y and z values");

                vertices.Add([
                    ParseFloat(parts[1], path, lineNumber),
                    ParseFloat(parts[2], path, lineNumber),
                    ParseFloat(parts[3], path, lineNumber)
                ]);
            }
            else if (parts[0] == "f")
            {
                if (parts.Length < 4)
                    throw new FormatException($"{path}({lineNumber}): face requires at least three vertices");

                int[] polygon = parts[1..]
                    .Select(token => ParseVertexIndex(token, vertices.Count, path, lineNumber))
                    .ToArray();

                // the renderer consumes faces directly - triangulating here also lets it handle OBJ polygons of any size while preserving their winding order
                for (int i = 1; i < polygon.Length - 1; i++)
                    faces.Add([polygon[0], polygon[i], polygon[i + 1], polygon[i + 1]]);
            }
        }

        if (vertices.Count == 0 || faces.Count == 0)
            throw new FormatException($"{path}: OBJ must contain at least one vertex and one face");

        _model = vertices.ToArray();
        Faces = faces.Select(indices => new Face(indices, this)).ToArray();
    }

    private static float ParseFloat(string value, string path, int lineNumber)
    {
        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        throw new FormatException($"{path}({lineNumber}): '{value}' is not a valid number");
    }

    private static int ParseVertexIndex(string token, int vertexCount, string path, int lineNumber)
    {
        // only the position index is used - texture/normal fields such as 12/4/7 are ignored.
        string positionIndex = token.Split('/')[0];
        if (!int.TryParse(positionIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out int objIndex) || objIndex == 0)
            throw new FormatException($"{path}({lineNumber}): '{token}' has an invalid vertex index");

        int index = objIndex > 0 ? objIndex - 1 : vertexCount + objIndex;
        if ((uint)index >= (uint)vertexCount)
            throw new FormatException($"{path}({lineNumber}): vertex index {objIndex} is out of range");

        return index;
    }

    public override IEnumerable<DrawCall> CollectFaces(Camera camera, IReadOnlyList<LightSource> lightSources)
    {
        Vector3f[] worldVertices = new Vector3f[_model.Length];
        Vector3f[] viewVertices = new Vector3f[_model.Length];
        Vector2f[] projectedVertices = new Vector2f[_model.Length];
        bool[] verticesInView = new bool[_model.Length];

        for (int i = 0; i < _model.Length; i++)
        {
            Vector3f modelPoint = new(_model[i][0], _model[i][1], _model[i][2]);
            worldVertices[i] = Util.ToWorld(Util.ToLocal(modelPoint, Scale, Rotation), Position);
            (viewVertices[i], verticesInView[i]) = Util.ToView(worldVertices[i], camera);

            if (verticesInView[i])
                projectedVertices[i] = Util.ToXY(viewVertices[i]);
        }

        foreach (Face face in Faces)
        {
            DrawCall? drawCall = face.TryGetDrawCall(
                viewVertices, worldVertices, projectedVertices, verticesInView,
                BaseShapeColor, camera, lightSources);

            if (drawCall is not null)
                yield return drawCall.Value;
        }
    }
}
