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

        var quad = new VertexArray(PrimitiveType.Quads, 4);

        quad[0] = new Vertex(p1 - normal, colorStart);
        quad[1] = new Vertex(p2 - normal, colorEnd);
        quad[2] = new Vertex(p2 + normal, colorEnd);
        quad[3] = new Vertex(p1 + normal, colorStart);

        return quad;
    }
}
