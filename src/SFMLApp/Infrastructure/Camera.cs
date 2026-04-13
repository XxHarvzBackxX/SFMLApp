using SFML.System;

namespace SFMLApp.Infrastructure;

public class Camera
{
    public Vector3f Position; // default: (0f, 0f, 0f)
    public Vector2f Rotation; // Heading & Pitch (X, Y); default: (0f, 0f)
}
