using SFML.System;
using SFMLApp.Utility;

namespace SFMLApp.Infrastructure;

public class Camera
{
    public Vector3f Position;
    public Vector2f Rotation; // X = pitch, Y = yaw

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
            float yaw = Rotation.Y;

            return Util.Normalize(new Vector3f(
                MathF.Cos(yaw),
                0f,
                MathF.Sin(yaw)
            ));
        }
    }

    public Vector3f Up => new(0f, 1f, 0f);
}