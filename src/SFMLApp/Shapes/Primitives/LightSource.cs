using SFML.Graphics;

namespace SFMLApp.Shapes.Primitives;

public class LightSource : Cube
{
    public float Intensity { get; set; } = 4.0f;
    public float MinimumBrightness { get; set; } = 0.1f;

    public LightSource(
        float posX = 0f,
        float posY = 0f,
        float posZ = 0f,
        float rotX = 0f,
        float rotY = 0f,
        float rotZ = 0f,
        float scale = 1f,
        Color? color = null)
        : base(posX, posY, posZ, rotX, rotY, rotZ, scale, color ?? Color.Blue)
    {
    }
}