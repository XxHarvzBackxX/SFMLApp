using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SFMLApp;

internal class Util
{
    public static CircleShape CircleFactory(float radius, Color fillColor, Vector2f position) => new CircleShape(radius) { FillColor = fillColor, Position = position };

    public static RectangleShape SolidLineFactory(Vector2f p1, Vector2f p2, float width, Color color)
    {
        var d = p1 - p2;
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
        var direction = p2 - p1;
        float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

        var unit = direction / length;

        var normal = new Vector2f(-unit.Y, unit.X) * (width / 2f);

        VertexArray quad = new(PrimitiveType.Quads, 4);

        quad[0] = new Vertex(p1 - normal, colorStart);
        quad[1] = new Vertex(p2 - normal, colorEnd);
        quad[2] = new Vertex(p2 + normal, colorEnd);
        quad[3] = new Vertex(p1 + normal, colorStart);

        return quad;
    }

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

    public static Vector3f MutateX(Vector3f point, float newX) => new(newX, point.Y, point.Z);

    public static Vector3f MutateY(Vector3f point, float newY) => new(point.X, newY, point.Z);

    public static Vector3f MutateZ(Vector3f point, float newZ) => new(point.X, point.Y, newZ);

    public static Vector3f IncrementMutateX(Vector3f point, float incX) => new(point.X + incX, point.Y, point.Z);

    public static Vector3f IncrementMutateY(Vector3f point, float incY) => new(point.X, point.Y + incY, point.Z);

    public static Vector3f IncrementMutateZ(Vector3f point, float incZ) => new(point.X, point.Y, point.Z + incZ);
}
