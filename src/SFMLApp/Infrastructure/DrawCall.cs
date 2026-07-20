using SFML.Graphics;

namespace SFMLApp.Infrastructure;

public readonly struct DrawCall
{
    /// <summary>
    /// Average view-space Z of the face vertices. More negative = further from camera.
    /// Used as the painter's algorithm sort key.
    /// </summary>
    public float Depth { get; }
    public Vertex Vertex0 { get; }
    public Vertex Vertex1 { get; }
    public Vertex Vertex2 { get; }
    public Vertex Vertex3 { get; }

    public DrawCall(float depth, Vertex vertex0, Vertex vertex1, Vertex vertex2, Vertex vertex3)
    {
        Depth = depth;
        Vertex0 = vertex0;
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
    }
}
