using SFML.System;
using SFMLApp.Utility;

namespace SFMLApp.Infrastructure;

public class Camera
{
    public Projection Projection = null!;
    public Vector3f Position;
    public Vector2f Rotation; // pitch (X), yaw (Y)

    public Vector3f Forward
    {
        get
        {
            float yaw = Rotation.Y;
            float pitch = Rotation.X;

            return Util.Normalize(new Vector3f(
                MathF.Sin(yaw) * MathF.Cos(pitch),
                -MathF.Sin(pitch),
                -MathF.Cos(yaw) * MathF.Cos(pitch)
            ));
        }
    }

    public Vector3f ForwardFlat
    {
        get
        {
            float yaw = Rotation.Y;

            return Util.Normalize(new Vector3f(
                MathF.Sin(yaw),
                0f,
                -MathF.Cos(yaw)
            ));
        }
    }

    public Vector3f Right
    {
        get
        {
            Vector3f forwardFlat = ForwardFlat;
            return new Vector3f(-forwardFlat.Z, 0f, forwardFlat.X);
        }
    }

    public Vector3f Up => new(0f, 1f, 0f);
}

public class Projection
{
    public float FieldOfView { get; set; } = 0.3f; // 108 deg
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 100f;
}
