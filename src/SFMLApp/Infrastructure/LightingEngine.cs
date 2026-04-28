using SFML.Graphics;
using SFML.System;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Utility;

namespace SFMLApp.Infrastructure;

public static class LightingEngine
{
    public static Color ComputeLitColor(
        Vector3f worldPoint,
        Vector3f normal,
        Color faceColor,
        IReadOnlyList<LightSource> lightSources)
    {
        float r = 0f, g = 0f, b = 0f;
        float ambient = 0f;

        foreach (LightSource light in lightSources)
        {
            ambient = Math.Max(ambient, light.MinimumBrightness);

            Vector3f toLight = light.Position - worldPoint;
            Vector3f toLightDir = Util.Normalize(toLight);
            float distance = Util.Magnitude(toLight);

            float diffuse = Math.Max(0f, Util.Dot(toLightDir, normal));

            float contribution = diffuse * light.Intensity / (distance * distance);

            r += (light.BaseShapeColor.R / 255f) * contribution;
            g += (light.BaseShapeColor.G / 255f) * contribution;
            b += (light.BaseShapeColor.B / 255f) * contribution;
        }

        return new Color(
            (byte)(faceColor.R * Math.Clamp(ambient + r, 0f, 1f)),
            (byte)(faceColor.G * Math.Clamp(ambient + g, 0f, 1f)),
            (byte)(faceColor.B * Math.Clamp(ambient + b, 0f, 1f)));
    }
}
