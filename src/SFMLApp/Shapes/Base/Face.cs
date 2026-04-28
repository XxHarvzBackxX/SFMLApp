using SFML.Graphics;
using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Base;

public class Face
{
    public SimpleShape Parent { get; private set; }
    private int[] _vertices;
    public Face(int[] vertices, SimpleShape parent)
    {
        _vertices = vertices;
        Parent = parent;
    }

    // Faces with a view-space centroid Z closer to the camera than this are clipped.
    private const float NearPlane = -0.1f;

    /// <summary>
    /// Attempts to produce a depth-keyed draw call for this face.
    /// Returns <see langword="null"/> when the face is culled (backface, behind camera, or on the near plane).
    /// </summary>
    public DrawCall? TryGetDrawCall(
        Vector3f[] viewVertices,
        Vector3f[] worldVertices,
        Vector2f[] projectedVertices,
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
            faceWorldVertices[i] = worldVertices[index];
            faceViewVertices[i] = viewVertices[index];
            faceProjectedVertices[i] = projectedVertices[index];
            i++;
        }

        // cull faces behind the camera or intersecting the near plane
        float centroidViewZ = 0f;
        foreach (Vector3f v in faceViewVertices) centroidViewZ += v.Z;
        centroidViewZ /= faceViewVertices.Length;
        if (centroidViewZ >= NearPlane) return null;

        // backface culling
        Vector3f toCamera = camera.Position - faceWorldVertices[0];
        Vector3f normal = Util.Cross(faceWorldVertices[0], faceWorldVertices[1], faceWorldVertices[2]);
        if (Util.Dot(toCamera, normal) <= 0f) return null;

        bool isEmissive = Parent is LightSource;
        Color[] vertexColors = new Color[_vertices.Length];

        if (!isEmissive)
        {
            for (int v = 0; v < _vertices.Length; v++)
                vertexColors[v] = LightingEngine.ComputeLitColor(faceWorldVertices[v], normal, faceColor, lightSources);
        }
        else
        {
            for (int v = 0; v < _vertices.Length; v++)
                vertexColors[v] = faceColor;
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

            Vector3f normalEnd = centroidView + normal / 2;
            Vector2f projectedNormalEnd = Util.ToXY(normalEnd);
            Util.GradientLine(projectedCentroid, projectedNormalEnd, Color.Green, Color.Red);
            Util.Circle(projectedNormalEnd, Color.Red, 5f);
        });
    }
}
