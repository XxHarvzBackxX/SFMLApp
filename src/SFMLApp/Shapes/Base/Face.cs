using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Base;

public class Face
{
    public Shape3D Parent { get; private set; }
    private int[] _vertices;
    public Face(int[] vertices, Shape3D parent)
    {
        _vertices = vertices;
        Parent = parent;
    }

    /// <summary>
    /// Attempts to produce a depth-keyed draw call for this face.
    /// Returns <see langword="null"/> when the face is outside the camera frustum or backface culled.
    /// </summary>
    public DrawCall? TryGetDrawCall(
        Vector3f[] viewVertices,
        Vector3f[] worldVertices,
        Vector2f[] projectedVertices,
        bool[] verticesInView,
        Color faceColor,
        Camera camera,
        IReadOnlyList<LightSource> lightSources)
    {
        Vector3f[] faceWorldVertices = new Vector3f[_vertices.Length];
        Vector3f[] faceViewVertices = new Vector3f[_vertices.Length];
        Vector2f[] faceProjectedVertices = new Vector2f[_vertices.Length];

        int i = 0;
        foreach (int index in _vertices)
        {
            if (!verticesInView[index]) return null;

            faceWorldVertices[i] = worldVertices[index];
            faceViewVertices[i] = viewVertices[index];
            faceProjectedVertices[i] = projectedVertices[index];
            i++;
        }

        // this remains the depth-sort key after the frustum check above
        float centroidViewZ = 0f;
        foreach (Vector3f v in faceViewVertices) centroidViewZ += v.Z;
        centroidViewZ /= faceViewVertices.Length;

        Vector3f faceNormal = Util.Cross(
            faceWorldVertices[0],
            faceWorldVertices[1],
            faceWorldVertices[2]);
        Vector3f toCamera = camera.Position - faceWorldVertices[0];
        if (Util.Dot(toCamera, faceNormal) <= 0f) return null;

        Vector3f[] normals = new Vector3f[_vertices.Length];
        for (int v = 0; v < _vertices.Length; v++)
            normals[v] = Parent.GetVertexNormal(_vertices[v], faceWorldVertices[v], faceNormal);

        bool isEmissive = Parent is LightSource;
        Color[] vertexColors = new Color[_vertices.Length];

        for (int v = 0; v < _vertices.Length; v++)
        {
            if (!isEmissive)
            {
                vertexColors[v] = LightingEngine.ComputeLitColor(
                    faceWorldVertices[v], normals[v], faceColor, lightSources);
            }
            else
            {
                vertexColors[v] = faceColor;
            }
        }

        return new DrawCall(centroidViewZ, () =>
        {
            Util.Quad(faceProjectedVertices, vertexColors);

            if (!Program.DebugView) return;

            for (int j = 0; j < faceProjectedVertices.Length; j++)
            {
                Vector2f point1 = faceProjectedVertices[j];
                Vector2f point2 = faceProjectedVertices[(j + 1) % faceProjectedVertices.Length]; // wrap around to 0 for i == length
                Util.GradientLine(point1, point2, Color.Blue, Color.Cyan);
            }

            Vector3f centroidView = Util.Centroid(faceViewVertices);
            Vector2f projectedCentroid = Util.ToXY(centroidView);
            Util.Circle(projectedCentroid, Color.Green, 5f);

            //Vector3f normalEnd = centroidView + normal / 2;
            //Vector2f projectedNormalEnd = Util.ToXY(normalEnd);
            //Util.GradientLine(projectedCentroid, projectedNormalEnd, Color.Green, Color.Red);
            //Util.Circle(projectedNormalEnd, Color.Red, 5f);
        });
    }
}
