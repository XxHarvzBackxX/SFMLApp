using SFML.Graphics;
using SFML.System;

namespace SFMLApp;

public class Cube
{
    public Vector3f Position { get; set; }
    public float Scale { get; set; }
    public Vector3f Rotation { get; set; }
    private int[][] _vertices { get; set; } = [];

    private static readonly int[][] _edges =
    [
        // back face edges
        [0, 1], [1, 2],
        [2, 3], [3, 0],
        // front face edges
        [4, 7], [7, 6],
        [6, 5], [5, 4],
        // back to front connecting edges
        [0, 4], [1, 7],
        [2, 6], [3, 5]
    ];

    public Cube(float posX = 0f, float posY = 0f, float posZ = 0f, float rotX = 0f, float rotY = 0f, float rotZ = 0f, float scale = 1f)
    {
        Position = new Vector3f(posX, posY, posZ);
        Rotation = new Vector3f(rotX, rotY, rotZ);
        Scale = scale;

        _vertices = [
            [ -1, -1, -1 ], // 0-left, top, back
            [ 1, -1, -1 ], // 1-right, top, back
            [ 1, 1, -1 ], // 2-right, bottom, back
            [ -1, 1, -1 ], // 3-left, bottom, back

            // reverse winding order to orient normals outwards for back-face culling
            [ -1, -1, 1 ], // 4-left, top, front
            [ -1, 1, 1 ], // 5-left, bottom, front
            [ 1, 1, 1 ], // 6-right, bottom, front
            [ 1, -1, 1 ], // 7-right, top, front
        ];
    }

    private const float NearPlane = -0.1f;

    public void Draw()
    {
        Vector2f[] projectedVertices = new Vector2f[_vertices.Length];
        Vector3f[] worldVertices = new Vector3f[_vertices.Length];

        for (int vertexIndex = 0; vertexIndex < _vertices.Length; vertexIndex++)
        {
            int[] vertexCoords = _vertices[vertexIndex];
            Vector3f modelSpacePoint = new Vector3f(vertexCoords[0], vertexCoords[1], vertexCoords[2]);
            Vector3f localSpacePoint = _toLocal(modelSpacePoint);
            worldVertices[vertexIndex] = _toWorld(localSpacePoint);
            projectedVertices[vertexIndex] = _toXY(worldVertices[vertexIndex]);
        }

        foreach (int[] edgeIndices in _edges)
        {
            int startVertexIndex = edgeIndices[0];
            int endVertexIndex = edgeIndices[1];

            // skip this edge if either vertex is behind the 'near plane'
            if (worldVertices[startVertexIndex].Z >= NearPlane || worldVertices[endVertexIndex].Z >= NearPlane)
                continue;

            // draw edges
            Vector2f edgeStart = projectedVertices[startVertexIndex];
            Vector2f edgeEnd = projectedVertices[endVertexIndex];
            VertexArray edgeLine = Util.GradientLineFactory(edgeStart, edgeEnd, 2f, Color.Blue, Color.Cyan);
            Program.Window!.Draw(edgeLine);
        }

        for (int vertexIndex = 0; vertexIndex < projectedVertices.Length; vertexIndex++)
        {
            // skip this vertex if behind the 'near plane'
            if (worldVertices[vertexIndex].Z >= NearPlane)
                continue;

            // draw vertices
            Vector2f vertexScreenPos = projectedVertices[vertexIndex] - new Vector2f(3f, 3f);
            CircleShape vertexDot = Util.CircleFactory(3f, Color.Red, vertexScreenPos);
            Program.Window!.Draw(vertexDot);
        }
    }

    private Vector3f _toLocal(Vector3f point)
    {
        point *= Scale;
        // TODO: rotate around xyz axes
        return point;
    }

    private Vector3f _toWorld(Vector3f localPoint) => localPoint + Position;

    private Vector2f _toXY(Vector3f worldPoint)
    {
        worldPoint.X /= worldPoint.Z;
        worldPoint.Y /= worldPoint.Z;

        Vector2f screenSize = new Vector2f(800, 600);

        worldPoint.X *= screenSize.X;
        worldPoint.Y *= screenSize.Y;

        worldPoint.X += screenSize.X / 2;
        worldPoint.Y += screenSize.Y / 2;

        return new Vector2f(worldPoint.X, worldPoint.Y);
    }

}
