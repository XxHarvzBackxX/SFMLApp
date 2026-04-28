using SFML.System;
using SFML.Graphics;

namespace SFMLApp.Shapes.Base;

public abstract class Shape3D
{
    private Vector3f _position;
    public ref Vector3f Position => ref _position;
    public float Scale { get; set; }
    private Vector3f _rotation;
    public ref Vector3f Rotation => ref _rotation;

    public Color BaseShapeColor { get; set; }

    public Face[] Faces { get; set; } = null!;

    /// <summary>
    /// Represents vertices
    /// </summary>
    protected int[][] _model { get; set; } = null!;
}
