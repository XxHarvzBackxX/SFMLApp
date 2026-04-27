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

    public void Draw(
        Vector3f[] viewVertices,
        Vector3f[] worldVertices,
        Vector2f[] projectedVertices,
        Color faceColor,
        Camera camera,
        LightSource lightSource)
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

        // calculate vector to camera
        Vector3f toCamera = camera.Position - faceWorldVertices[0];

        // calculate normal
        Vector3f normal = Util.Cross(faceWorldVertices[0], faceWorldVertices[1], faceWorldVertices[2]);

        // dot product of normal & toCamera
        float dp = Util.Dot(toCamera, normal);

        if (dp <= 0f) return; // cull backfaces

        Vector3f centroidWorld = Util.Centroid(faceWorldVertices);

        float lightStrength = 1f;

        if (Parent != lightSource)
        {
            Vector3f toLightSource = lightSource.Position - faceWorldVertices[0];

            float lightDotProduct = Util.Dot(Util.Normalize(toLightSource), normal);
            float distanceToLightSource = Util.Magnitude(toLightSource);

            lightStrength = lightDotProduct * lightSource.Intensity / distanceToLightSource;
            lightStrength = Math.Max(lightSource.MinimumBrightness, lightStrength);
            lightStrength = Math.Min(lightStrength, 1.0f);

            Color lightColor = lightSource.BaseShapeColor;
            Color litFaceColor = Util.Attenuate(Util.Mix(faceColor, lightColor), lightStrength);

            Util.Quad(faceProjectedVertices, litFaceColor);
        }
        else
        {
            Util.Quad(faceProjectedVertices, faceColor);
        }

        if (!Program.DebugView) return;


        for (int j = 0; j < _vertices.Length; j++)
        {
            Vector2f point1 = faceProjectedVertices[j];
            Vector2f point2 = faceProjectedVertices[(j + 1) % _vertices.Length]; // wrap around to 0 for i == length

            Util.GradientLine(point1, point2, Color.Blue, Color.Cyan);
        }

        Vector3f centroidView = Util.Centroid(faceViewVertices);

        Vector2f projectedCentroid = Util.ToXY(centroidView);

        Util.Circle(projectedCentroid, Color.Green, 5f);


        Vector3f normalEnd = centroidView - normal / 2;
        Vector2f projectedNormalEnd = Util.ToXY(normalEnd);

        Util.GradientLine(projectedCentroid, projectedNormalEnd, Color.Green, Color.Red);
        Util.Circle(projectedNormalEnd, Color.Red, 5f);
    }
}
