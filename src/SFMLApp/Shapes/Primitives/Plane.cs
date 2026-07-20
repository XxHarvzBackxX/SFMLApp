using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;
using SFMLApp.Shapes.LandscapeFeatures;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Primitives;

public class Plane : Shape3D
{
    public static readonly Color ONE_COLOR = new(64, 64, 64);
    public static readonly Color TWO_COLOR = new(128, 128, 128);

    private readonly Color[] _faceColors;

    public Plane(int divisions = 10, int scale = 1, float height = 0f, params Terrain[] terrains)
    {
        Position = new Vector3f();
        Rotation = new Vector3f();
        Scale = scale;
        BaseShapeColor = ONE_COLOR;

        int pointsPerSide = divisions + 1;
        _model = new float[pointsPerSide * pointsPerSide][];

        float offset = -divisions / 2f;
        int index = 0;
        for (int z = 0; z <= divisions; z++)
        {
            for (int x = 0; x <= divisions; x++)
                _model[index++] = [x + offset, height, z + offset];
        }

        if (terrains.Length > 0)
            ProcessTerrain(terrains);

        Faces = new Face[divisions * divisions];
        _faceColors = new Color[Faces.Length];

        index = 0;
        for (int z = 0; z < divisions; z++)
        {
            for (int x = 0; x < divisions; x++)
            {
                int nearLeft = z * pointsPerSide + x;
                int nearRight = nearLeft + 1;
                int farLeft = nearLeft + pointsPerSide;
                int farRight = farLeft + 1;

                // this winding points toward negative Y, which is up in the scene
                Faces[index] = new Face([nearLeft, nearRight, farRight, farLeft], this);
                _faceColors[index] = ((x + z) % 2 == 0) ? ONE_COLOR : TWO_COLOR;
                index++;
            }
        }
    }

    private void ProcessTerrain(Terrain[] terrains)
    {
        for (int i = 0; i < _model.Length; i++)
        {
            Vector3f point = Util.VectorFromArray(_model[i]);
            float totalHeight = 0f;

            foreach (Terrain terrain in terrains)
                totalHeight += terrain.HeightAt(point);

            _model[i][1] += totalHeight / terrains.Length;
        }
    }

    public override IEnumerable<DrawCall> CollectFaces(
        Camera camera,
        IReadOnlyList<LightSource> lightSources)
    {
        Vector3f[] worldVertices = new Vector3f[_model.Length];
        Vector3f[] viewVertices = new Vector3f[_model.Length];
        Vector2f[] projectedVertices = new Vector2f[_model.Length];
        bool[] verticesInView = new bool[_model.Length];

        for (int i = 0; i < _model.Length; i++)
        {
            Vector3f modelPoint = Util.VectorFromArray(_model[i]);
            Vector3f localPoint = Util.ToLocal(modelPoint, Scale, Rotation);
            worldVertices[i] = Util.ToWorld(localPoint, Position);
            (viewVertices[i], verticesInView[i]) = Util.ToView(worldVertices[i], camera);

            if (verticesInView[i])
                projectedVertices[i] = Util.ToXY(viewVertices[i]);
        }

        for (int i = 0; i < Faces.Length; i++)
        {
            DrawCall? drawCall = Faces[i].TryGetDrawCall(
                viewVertices,
                worldVertices,
                projectedVertices,
                verticesInView,
                _faceColors[i],
                camera,
                lightSources);

            if (drawCall is not null)
                yield return drawCall.Value;
        }
    }
}
