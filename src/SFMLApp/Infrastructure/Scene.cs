using SFMLApp.Shapes.Base;
using SFMLApp.Shapes.Primitives;

namespace SFMLApp.Infrastructure;

public class Scene
{
    private readonly List<SimpleShape> _objects = [];
    private readonly List<LightSource> _lightSources = [];

    public IReadOnlyList<LightSource> LightSources => _lightSources;
    public IReadOnlyList<SimpleShape> Objects => _objects;

    public Scene(IEnumerable<LightSource>? lightSources = null, IEnumerable<SimpleShape>? objects = null)
    {
        foreach (LightSource light in lightSources ?? [])
            AddLight(light);

        foreach (SimpleShape obj in objects ?? [])
            Add(obj);
    }

    public void Add(SimpleShape shape) => _objects.Add(shape);
    public void Remove(SimpleShape shape) => _objects.Remove(shape);

    public void AddLight(LightSource lightSource)
    {
        _lightSources.Add(lightSource);
        _objects.Add(lightSource);
    }

    public void RemoveLight(LightSource lightSource)
    {
        _lightSources.Remove(lightSource);
        _objects.Remove(lightSource);
    }

    public void Render(Camera camera)
    {
        List<DrawCall> drawCalls = [];

        foreach (SimpleShape obj in _objects)
            drawCalls.AddRange(obj.CollectFaces(camera, _lightSources));

        // sort back-to-front (most negative Z is furthest away)
        drawCalls.Sort(static (a, b) => a.Depth.CompareTo(b.Depth));

        foreach (DrawCall drawCall in drawCalls)
            drawCall.Draw();
    }
}
