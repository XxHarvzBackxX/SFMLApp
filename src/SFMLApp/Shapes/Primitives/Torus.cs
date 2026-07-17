using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Primitives;

public class Torus : Shape3D
{
    private Vector3f[] _modelNormals = null!;

    public float Radius { get; }
    public float Thickness { get; }
    public int Segments { get; }
    public int TubeSegments { get; }

    public Torus(
        float posX = 0f,
        float posY = 0f,
        float posZ = 0f,
        float rotX = 0f,
        float rotY = 0f,
        float rotZ = 0f,
        float scale = 1f,
        float radius = 2f,
        float thickness = 0.5f,
        int segments = 48,
        int tubeSegments = 16,
        Color? baseShapeColor = null)
    {
        Position = new Vector3f(posX, posY, posZ);
        Rotation = new Vector3f(rotX, rotY, rotZ);
        Scale = scale;
        BaseShapeColor = baseShapeColor ?? new Color(255, 255, 255);
        Radius = radius;
        Thickness = thickness;
        Segments = Math.Max(3, segments);
        TubeSegments = Math.Max(3, tubeSegments);

        _initModels();
    }

    private void _initModels()
    {
        _model = new float[Segments * TubeSegments][];
        _modelNormals = new Vector3f[_model.Length];

        int index = 0;
        for (int segment = 0; segment < Segments; segment++)
        {
            float torusAngle = segment * 2f * MathF.PI / Segments;
            float torusCos = MathF.Cos(torusAngle);
            float torusSin = MathF.Sin(torusAngle);

            for (int tubeSegment = 0; tubeSegment < TubeSegments; tubeSegment++)
            {
                float tubeAngle = tubeSegment * 2f * MathF.PI / TubeSegments;
                float tubeCos = MathF.Cos(tubeAngle);
                float tubeSin = MathF.Sin(tubeAngle);

                float radialDistance = Radius + Thickness * tubeCos;
                _model[index] = [
                    radialDistance * torusCos,
                    Thickness * tubeSin,
                    radialDistance * torusSin
                ];

                _modelNormals[index] = new Vector3f(
                    tubeCos * torusCos,
                    tubeSin,
                    tubeCos * torusSin);

                index++;
            }
        }

        List<Face> faces = [];
        for (int segment = 0; segment < Segments; segment++)
        {
            int nextSegment = (segment + 1) % Segments;

            for (int tubeSegment = 0; tubeSegment < TubeSegments; tubeSegment++)
            {
                int nextTubeSegment = (tubeSegment + 1) % TubeSegments;

                int current = segment * TubeSegments + tubeSegment;
                int currentNextTube = segment * TubeSegments + nextTubeSegment;
                int next = nextSegment * TubeSegments + tubeSegment;
                int nextNextTube = nextSegment * TubeSegments + nextTubeSegment;

                faces.Add(new Face([current, currentNextTube, nextNextTube, next], this));
            }
        }

        Faces = faces.ToArray();
    }

    public override Vector3f GetVertexNormal(int vertexIndex, Vector3f worldPoint, Vector3f faceNormal)
    {
        Vector3f rotatedNormal = Util.ToLocal(_modelNormals[vertexIndex], 1f, Rotation);
        return Util.Normalize(rotatedNormal);
    }

    public override IEnumerable<DrawCall> CollectFaces(Camera camera, IReadOnlyList<LightSource> lightSources)
    {
        Vector3f[] worldVertices = new Vector3f[_model.Length];
        Vector3f[] viewVertices = new Vector3f[_model.Length];
        bool[] verticesInView = new bool[_model.Length];

        for (int vertexIndex = 0; vertexIndex < _model.Length; vertexIndex++)
        {
            float[] vertexCoords = _model[vertexIndex];

            Vector3f modelSpacePoint = new(vertexCoords[0], vertexCoords[1], vertexCoords[2]);
            Vector3f localSpacePoint = Util.ToLocal(modelSpacePoint, Scale, Rotation);

            worldVertices[vertexIndex] = Util.ToWorld(localSpacePoint, Position);
            (viewVertices[vertexIndex], verticesInView[vertexIndex]) =
                Util.ToView(worldVertices[vertexIndex], camera);
        }

        Vector2f[] projectedVertices = new Vector2f[_model.Length];
        for (int vertexIndex = 0; vertexIndex < _model.Length; vertexIndex++)
        {
            if (verticesInView[vertexIndex])
                projectedVertices[vertexIndex] = Util.ToXY(viewVertices[vertexIndex]);
        }

        foreach (Face face in Faces)
        {
            DrawCall? drawCall = face.TryGetDrawCall(
                viewVertices,
                worldVertices,
                projectedVertices,
                verticesInView,
                BaseShapeColor,
                camera,
                lightSources);

            if (drawCall is not null)
                yield return drawCall;
        }
    }
}
