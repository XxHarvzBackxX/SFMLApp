using SFML.Graphics;
using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;

namespace SFMLApp.Utility;

internal class Util
{
    public static CircleShape CircleFactory(float radius, Color fillColor, Vector2f position) => new CircleShape(radius) { FillColor = fillColor, Position = position, Origin = new Vector2f(radius, radius) };

    public static RectangleShape SolidLineFactory(Vector2f p1, Vector2f p2, float width, Color color)
    {
        Vector2f d = p1 - p2;
        float len = (float)Math.Sqrt(d.X * d.X + d.Y * d.Y);
        float angle = (float)(Math.Atan2(d.Y, d.X) * 180.0 / Math.PI);

        return new RectangleShape(new Vector2f(len, width))
        {
            Position = p1,
            FillColor = color,
            Rotation = angle,
            Origin = new Vector2f(0, width / 2f)
        };
    }

    public static VertexArray GradientLineFactory(
        Vector2f p1,
        Vector2f p2,
        float width,
        Color colorStart,
        Color colorEnd)
    {
        Vector2f direction = p2 - p1;
        float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

        Vector2f unit = direction / length;

        Vector2f normal = new Vector2f(-unit.Y, unit.X) * (width / 2f);

        VertexArray quad = new(PrimitiveType.Quads, 4);

        quad[0] = new Vertex(p1 - normal, colorStart);
        quad[1] = new Vertex(p2 - normal, colorEnd);
        quad[2] = new Vertex(p2 + normal, colorEnd);
        quad[3] = new Vertex(p1 + normal, colorStart);

        return quad;
    }

    public static void Line(Vector2f p1, Vector2f p2, Color color, float width = 3f) => Program.Window!.Draw(SolidLineFactory(p1, p2, width, color));

    public static void Quad(Vector2f[] points, Color color)
    {
        VertexArray quad = new VertexArray(PrimitiveType.Quads);

        foreach (Vector2f p in points)
        {
            quad.Append(new Vertex(p, color));
        }

        Program.Window!.Draw(quad);
    }

    public static void Quad(Vector2f[] points, Color[] colors)
    {
        VertexArray quad = new VertexArray(PrimitiveType.Quads);

        for (int i = 0; i < points.Length; i++)
        {
            quad.Append(new Vertex(points[i], colors[i]));
        }

        Program.Window!.Draw(quad);
    }

    public static void GradientLine(Vector2f p1, Vector2f p2, Color color1, Color color2, float width = 3f) => Program.Window!.Draw(GradientLineFactory(p1, p2, width, color1, color2));

    public static void Circle(Vector2f position, Color fillColor, float radius = 3f) => Program.Window!.Draw(CircleFactory(radius, fillColor, position));

    public static Vector3f RotateY(Vector3f point, float theta)
    {
        float cosTheta = MathF.Cos(theta);
        float sinTheta = MathF.Sin(theta);

        float x = point.X * cosTheta - point.Z * sinTheta;
        float z = point.X * sinTheta + point.Z * cosTheta;

        return new(x, point.Y, z);
    }

    public static Vector3f RotateX(Vector3f point, float theta)
    {
        float cosTheta = MathF.Cos(theta);
        float sinTheta = MathF.Sin(theta);

        float y = point.Y * cosTheta - point.Z * sinTheta;
        float z = point.Y * sinTheta + point.Z * cosTheta;

        return new(point.X, y, z);
    }

    public static Vector3f RotateZ(Vector3f point, float theta)
    {
        float cosTheta = MathF.Cos(theta);
        float sinTheta = MathF.Sin(theta);

        float x = point.X * cosTheta - point.Y * sinTheta;
        float y = point.X * sinTheta + point.Y * cosTheta;

        return new(x, y, point.Z);
    }

    public static Vector3f ToLocal(Vector3f point, float scale, Vector3f rotation)
    {
        point *= scale;

        Vector3f rY = RotateY(point, rotation.Y);
        Vector3f rX = RotateX(rY, rotation.X);
        Vector3f rZ = RotateZ(rX, rotation.Z);

        return rZ;
    }

    public static Vector3f ToWorld(Vector3f localPoint, Vector3f position) => localPoint + position;

    public static Vector3f ToView(Vector3f worldPoint, Camera camera)
    {
        Vector3f translated = worldPoint - camera.Position;

        return ToLocal(
            translated,
            1f,
            new Vector3f(
                -camera.Rotation.X,
                -camera.Rotation.Y,
                0f));
    }

    public static Vector2f ToXY(Vector3f worldPoint)
    {
        worldPoint.X /= -worldPoint.Z;
        worldPoint.Y /= -worldPoint.Z;

        Vector2u screenSize = Program.Window!.Size;

        worldPoint.X *= screenSize.X;
        worldPoint.Y *= screenSize.Y;

        worldPoint.X += screenSize.X / 2;
        worldPoint.Y += screenSize.Y / 2;

        return new Vector2f(worldPoint.X, worldPoint.Y);
    }

    public static Vector3f Centroid(params Vector3f[] vertices)
    {
        float sumX = vertices.Sum(v => v.X);
        float sumY = vertices.Sum(v => v.Y);
        float sumZ = vertices.Sum(v => v.Z);

        sumX /= vertices.Length;
        sumY /= vertices.Length;
        sumZ /= vertices.Length;

        return new(sumX, sumY, sumZ);
    }

    public static Vector3f Normalize(Vector3f vector)
    {
        float mag = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);

        return vector / mag;
    }

    public static Vector3f Cross(Vector3f p1, Vector3f p2, Vector3f p3)
    {
        Vector3f v1 = p2 - p1;
        Vector3f v2 = p3 - p1;
        return Normalize(new(
            v1.Y * v2.Z - v1.Z * v2.Y,
            v1.Z * v2.X - v1.X * v2.Z,
            v1.X * v2.Y - v1.Y * v2.X
        ));
    }

    public static float Dot(Vector3f p1, Vector3f p2)
    {
        return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
    }

    public static Color Attenuate(Color color, float strength)
    {
        strength = Math.Clamp(strength, 0.0f, 1.0f);
        byte R = (byte)(color.R * strength);
        byte G = (byte)(color.G * strength);
        byte B = (byte)(color.B * strength);

        return new Color(R, G, B);
    }

    public static Color Mix(Color c1, Color c2, float bias)
    {
        bias = Math.Clamp(bias, 0.0f, 1.0f);
        byte R = (byte)(c1.R + (c2.R - c1.R) * bias);
        byte G = (byte)(c1.G + (c2.G - c1.G) * bias);
        byte B = (byte)(c1.B + (c2.B - c1.B) * bias);

        return new Color(R, G, B);
    }

    public static Color Mix(Color c1, Color c2)
    {
        byte R = (byte)((c1.R + c2.R) / 2.0f);
        byte G = (byte)((c1.G + c2.G) / 2.0f);
        byte B = (byte)((c1.B + c2.B) / 2.0f);

        return new Color(R, G, B);
    }

    public static float Magnitude(Vector3f vector) => MathF.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
}
