using SFML.Graphics;
using SFMLApp.Shapes.Base;
using SFMLApp.Shapes.Primitives;

namespace SFMLApp.Infrastructure;

public class Scene
{
    private readonly List<Shape3D> _objects = [];
    private readonly List<LightSource> _lightSources = [];
    private readonly List<ISceneUpdatable> _updatables = [];
    private readonly List<DrawCall> _drawCalls = [];

    public IReadOnlyList<LightSource> LightSources => _lightSources;
    public IReadOnlyList<Shape3D> Objects => _objects;
    public IReadOnlyList<ISceneUpdatable> Updatables => _updatables;

    public Scene(
        IEnumerable<LightSource>? lightSources = null,
        IEnumerable<Shape3D>? objects = null,
        IEnumerable<ISceneUpdatable>? updatables = null)
    {
        foreach (LightSource light in lightSources ?? [])
            AddLight(light);

        foreach (Shape3D obj in objects ?? [])
            Add(obj);

        foreach (ISceneUpdatable updatable in updatables ?? [])
            AddUpdatable(updatable);
    }

    public void Add(Shape3D shape) => _objects.Add(shape);
    public void Remove(Shape3D shape) => _objects.Remove(shape);

    public void AddUpdatable(ISceneUpdatable updatable) => _updatables.Add(updatable);
    public void RemoveUpdatable(ISceneUpdatable updatable) => _updatables.Remove(updatable);

    public void Update(float deltaTime)
    {
        // register hierarchical motion from parent to child
        foreach (ISceneUpdatable updatable in _updatables)
            updatable.Update(deltaTime);
    }

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
        _drawCalls.Clear();

        foreach (Shape3D obj in _objects)
            _drawCalls.AddRange(obj.CollectFaces(camera, _lightSources));

        // sort back-to-front (most negative Z is furthest away)
        _drawCalls.Sort(static (a, b) => a.Depth.CompareTo(b.Depth));

        // submit every depth-sorted quad in one native SFML draw call
        using VertexArray vertices = new(PrimitiveType.Quads, (uint)(_drawCalls.Count * 4));
        uint vertexIndex = 0;
        foreach (DrawCall drawCall in _drawCalls)
        {
            vertices[vertexIndex++] = drawCall.Vertex0;
            vertices[vertexIndex++] = drawCall.Vertex1;
            vertices[vertexIndex++] = drawCall.Vertex2;
            vertices[vertexIndex++] = drawCall.Vertex3;
        }

        Program.Window!.Draw(vertices);

        if (Program.DebugView)
        {
            foreach (DrawCall drawCall in _drawCalls)
            {
                Vertex[] quad = [drawCall.Vertex0, drawCall.Vertex1, drawCall.Vertex2, drawCall.Vertex3];
                for (int i = 0; i < quad.Length; i++)
                    Utility.Util.GradientLine(quad[i].Position, quad[(i + 1) % quad.Length].Position, Color.Blue, Color.Cyan);

                SFML.System.Vector2f centroid =
                    (quad[0].Position + quad[1].Position + quad[2].Position + quad[3].Position) / 4f;
                Utility.Util.Circle(centroid, Color.Green, 5f);
            }
        }
    }
}
