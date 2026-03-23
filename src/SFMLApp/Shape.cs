using SFML.System;

namespace SFMLApp;

public abstract class Shape
{
    public Vector3f Position { get; set; }
    public float Scale { get; set; }
    public Vector3f Rotation { get; set; }

    public Face[] Faces { get; set; }

    /// <summary>
    /// Represents vertices
    /// </summary>
    protected int[][] _model { get; set; }

    protected Vector3f _toLocal(Vector3f point)
    {
        point *= Scale;

        Vector3f rY = Util.RotateY(point, Rotation.Y);
        Vector3f rX = Util.RotateX(rY, Rotation.X);
        Vector3f rZ = Util.RotateZ(rX, Rotation.Z);

        return rZ;
    }

    protected Vector3f _toWorld(Vector3f localPoint) => localPoint + Position;

    protected Vector2f _toXY(Vector3f worldPoint)
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
