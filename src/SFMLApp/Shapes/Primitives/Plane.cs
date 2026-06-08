using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.LandscapeFeatures;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Primitives;

public class Plane
{
    public static readonly Color ONE_COLOR = new SFML.Graphics.Color(64, 64, 64);
    public static readonly Color TWO_COLOR = new SFML.Graphics.Color(128, 128, 128);

    private Vector3f[] _model;
    private int _scale;
    private float _height;
    private int _divisions;
    private Random _rand = new Random();

    public Plane(int divisions = 10, int scale = 1, float height = 0f, params Terrain[] terrains)
    {
        _scale = scale;
        _height = height;
        _divisions = divisions;
        _model = new Vector3f[(_divisions + 1) * (_divisions + 1)];

        float offset = -_divisions / 2f;
        int index = 0;

        for (int z = 0; z <= _divisions; z += _scale)
        {
            for (int x = 0; x <= _divisions; x += _scale)
            {
                _model[index] = new Vector3f(
                    (x + offset) * _scale,
                    _height, // + _rand.Next(-2, 2),
                    (z + offset) * _scale);

                index++;
            }
        }

        if (terrains is not null && terrains.Length > 0) ProcessTerrain(terrains);
    }

    private void ProcessTerrain(Terrain[] terrains)
    {
        for (int i = 0; i < _model.Length; i++)
        {
            List<float> deltaHeights = new List<float>();

            foreach (Terrain t in terrains)
            {
                deltaHeights.Add(t.HeightAt(_model[i]));
            }

            _model[i].Y += deltaHeights.Sum() / deltaHeights.Count;
        }
    }

    public void Draw(Camera camera)
    {
        Vector2f[] projectedPoints = Util.ProjectPoints(_model, _scale, new Vector3f(0, 0, 0), new Vector3f(0, 0, 0), camera);

        int step = _divisions + 1;
        for (int z = 0; z < _divisions; z++)
        {
            for (int x = 0; x < _divisions; x++)
            {
                int ne = z * step + x;
                int nw = ne + 1;
                int sw = nw + _divisions;
                int se = sw + 1;

                Vector2f neP = projectedPoints[ne];
                Vector2f nwP = projectedPoints[nw];
                Vector2f swP = projectedPoints[sw];
                Vector2f seP = projectedPoints[se];

                Util.Quad(
                    ((x + z) % 2 == 0) ? ONE_COLOR : TWO_COLOR,
                    neP,
                    nwP,
                    seP,
                    swP);
            }
        }
    }
}
