using SFML.Graphics;
using SFML.System;
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

        for (int j = 0; j < _vertices.Length; j++)
        {
            Vector2f point1 = faceProjectedVertices[j];
            Vector2f point2 = faceProjectedVertices[(j + 1) % _vertices.Length]; // wrap around to 0 for i == length

            Util.GradientLine(point1, point2, Color.Blue, Color.Cyan);
            //Util.Circle(point1, Color.Red);
        }

        Vector3f centroid = Util.Centroid(faceWorldVertices);
        Vector2f projectedCentroid = Util.ToXY(centroid);

        Util.Circle(projectedCentroid, Color.Green, 5f);

        Vector3f normalStart = Util.Cross(faceWorldVertices[0], faceWorldVertices[1], faceWorldVertices[2]); // arbit scale
        Vector3f normalEnd = centroid + normalStart / 2;

        Vector2f projectedNormalEnd = Util.ToXY(normalEnd);

        Util.GradientLine(projectedCentroid, projectedNormalEnd, Color.Green, Color.Red);
        Util.Circle(projectedNormalEnd, Color.Red, 5f);
    }
}
