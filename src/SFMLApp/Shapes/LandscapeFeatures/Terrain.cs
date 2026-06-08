using SFML.System;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.LandscapeFeatures;

public class Terrain
{
    public Vector3f Position;
    public float Radius;

    public Terrain(Vector3f pos, float radius)
    {
        Position = pos;
        Radius = radius;
    }

    public float HeightAt(Vector3f point)
    {
        float flatDistance = Util.FlatDistance(Position, point);

        if (flatDistance >= Radius) return 0f;

        return Util.SmootherStep(Position.Y, point.Y, flatDistance / Radius);
    }
}
