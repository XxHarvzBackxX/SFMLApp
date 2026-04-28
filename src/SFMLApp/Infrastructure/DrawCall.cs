namespace SFMLApp.Infrastructure;

public sealed class DrawCall
{
    /// <summary>
    /// Average view-space Z of the face vertices. More negative = further from camera.
    /// Used as the painter's algorithm sort key.
    /// </summary>
    public float Depth { get; }
    public Action Draw { get; }

    public DrawCall(float depth, Action draw)
    {
        Depth = depth;
        Draw = draw;
    }
}
