using SFML.Graphics;
using SFML.System;

namespace SFMLApp;

public class Face
{
    public Shape Parent { get; private set; }
    private int[] _vertices;
    public Face(int[] vertices, Shape parent)
    {
        _vertices = vertices;
        Parent = parent;
    }

    public void Draw(Vector3f[] worldVertices, Vector2f[] projectedVertices)
    {
        Vector3f[] faceWorldVertices = new Vector3f[_vertices.Length];
        Vector2f[] faceProjectedVertices = new Vector2f[_vertices.Length];

        int i = 0;
        foreach (int index in _vertices)
        {
            faceWorldVertices[i] = worldVertices[index];
            faceProjectedVertices[i] = projectedVertices[index];
            i++;
        }

        for (int j = 0; j < _vertices.Length - 1; j++)
        {
            Vector2f point1 = faceProjectedVertices[j];
            Vector2f point2 = faceProjectedVertices[(j + 1) % _vertices.Length]; // wrap around to 0 for i == length

            VertexArray edgeLine = Util.GradientLineFactory(point1, point2, 2f, Color.Blue, Color.Cyan);
            Program.Window!.Draw(edgeLine);
        }
    }
}
