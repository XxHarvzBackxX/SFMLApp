using SFML.System;
using SFMLApp.Shapes.Base;

namespace SFMLApp.Infrastructure;

public enum OrbitAxis
{
    X,
    Y,
    Z
}

public sealed class Orbit : ISceneUpdatable
{
    private float _elapsedTime;

    public Shape3D Orbiter { get; }
    public Shape3D Target { get; }
    public OrbitAxis Axis { get; set; } = OrbitAxis.Y;
    public float Radius { get; set; } = 5f;
    public float AngularSpeed { get; set; } = 1f;
    public float PhaseOffset { get; set; }
    public float VerticalOscillationAmplitude { get; set; }
    public float VerticalOscillationSpeed { get; set; } = 1f;
    public float VerticalOscillationPhaseOffset { get; set; }

    public Orbit(Shape3D orbiter, Shape3D target)
    {
        Orbiter = orbiter;
        Target = target;
    }

    public void Update(float deltaTime)
    {
        _elapsedTime += deltaTime;

        float angle = _elapsedTime * AngularSpeed + PhaseOffset;
        float cosineOffset = MathF.Cos(angle) * Radius;
        float sineOffset = MathF.Sin(angle) * Radius;
        Vector3f centre = Target.Position;

        Orbiter.Position = Axis switch
        {
            OrbitAxis.X => new Vector3f(centre.X, centre.Y + cosineOffset, centre.Z + sineOffset),
            OrbitAxis.Y => new Vector3f(centre.X + cosineOffset, centre.Y, centre.Z + sineOffset),
            OrbitAxis.Z => new Vector3f(centre.X + cosineOffset, centre.Y + sineOffset, centre.Z),
            _ => throw new InvalidOperationException("Unsupported orbit axis")
        };

        Orbiter.Position.Y += MathF.Sin(
            _elapsedTime * VerticalOscillationSpeed + VerticalOscillationPhaseOffset)
            * VerticalOscillationAmplitude;
    }
}
