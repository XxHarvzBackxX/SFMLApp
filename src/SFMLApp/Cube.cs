using SFML.Graphics;
using SFML.System;

namespace SFMLApp;

public class Cube
{
    public Vector3f Position { get; set; }
    public float Scale { get; set; }
    public Vector3f Rotation { get; set; }
    private int[][] _model { get; set; } = [];

    public Cube(float pX = 0f, float pY = 0f, float pZ = 0f, float rX = 0f, float rY = 0f, float rZ = 0f, float scale = 1f)
    {
        Position = new Vector3f(pX, pY, pZ);
        Rotation = new Vector3f(rX, rY, rZ);
        Scale = scale;

        _model = [
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

    public void Draw()
    {
        foreach (int[]? model in _model)
        {
            if (model is null) throw new NullReferenceException();

            Vector3f point = new Vector3f(model[0], model[1], model[2]);
            Vector3f localPoint = _toLocal(point);
            Vector3f worldPoint = _toWorld(localPoint);
            Vector2f xYPoint = _toXY(worldPoint);

            CircleShape dot = new CircleShape(2f)
            {
                Position = xYPoint,
                FillColor = Color.Red
            };

            Program.Window!.Draw(dot);
        }
    }

    private Vector3f _toLocal(Vector3f p)
    {
        p *= Scale;
        // TODO: rotate around xyz axes
        return p;
    }

    private Vector3f _toWorld(Vector3f p) => p + Position;

    private Vector2f _toXY(Vector3f p)
    {
        p.X /= p.Z; // x = x/z
        p.Y /= p.Z; // y = y/z

        Vector2f vSize = new Vector2f(800, 600);

        p.X *= vSize.X; // * width
        p.Y *= vSize.Y; // * height

        p.X += vSize.X / 2; // center-ish
        p.Y += vSize.Y / 2; // ^

        return new Vector2f(p.X, p.Y);
    }

}
