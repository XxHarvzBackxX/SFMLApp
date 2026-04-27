using SFML.Graphics;
using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Primitives;

public class Cube : SimpleShape
{
    public Cube(float posX = 0f, float posY = 0f, float posZ = 0f, float rotX = 0f, float rotY = 0f, float rotZ = 0f, float scale = 1f, Color? baseShapeColor = null)
    {
        Position = new Vector3f(posX, posY, posZ);
        Rotation = new Vector3f(rotX, rotY, rotZ);
        Scale = scale;
        BaseShapeColor = baseShapeColor ?? new Color(255, 255, 255);

        _initModels();
    }

    private void _initModels()
    {
        _model = [
            [ -1, -1, -1 ], // 0-left, top, back
            [ 1, -1, -1 ], // 1-right, top, back
            [ 1, 1, -1 ], // 2-right, bottom, back
            [ -1, 1, -1 ], // 3-left, bottom, back

            // reverse winding order to orient normals outwards for back-face culling
            [ -1, -1, 1 ], // 4-left, top, front
            [ -1, 1, 1 ], // 5-left, bottom, front
            [ 1, 1, 1 ], // 6-right, bottom, front
            [ 1, -1, 1 ], // 7-right, top, front
        ];

        Faces = [
            new([7, 6, 5, 4], this), // 0-front (near) face -- tlf, blf, brf, trf
            new([1, 0, 3, 2], this), // 1-back (far) face -- trb, tlb, blb, brb
            new([7, 1, 2, 6], this), // 2-right-hand face -- 
            new([0, 4, 5, 3], this), // 3-left-hand face 
            new([0, 1, 7, 4], this), // 4-top face
            new([5, 6, 2, 3], this) // 5-bottom face
        ];
    }

    public void Draw(Camera camera, LightSource lightSource)
    {
        Vector2f[] projectedVertices = new Vector2f[_model.Length];
        Vector3f[] worldVertices = new Vector3f[_model.Length];
        Vector3f[] viewVertices = new Vector3f[_model.Length];

        for (int vertexIndex = 0; vertexIndex < _model.Length; vertexIndex++)
        {
            int[] vertexCoords = _model[vertexIndex];

            Vector3f modelSpacePoint = new(vertexCoords[0], vertexCoords[1], vertexCoords[2]);
            Vector3f localSpacePoint = Util.ToLocal(modelSpacePoint, Scale, Rotation);

            worldVertices[vertexIndex] = Util.ToWorld(localSpacePoint, Position);
            viewVertices[vertexIndex] = Util.ToView(worldVertices[vertexIndex], camera);
            projectedVertices[vertexIndex] = Util.ToXY(viewVertices[vertexIndex]);
        }

        foreach (Face face in Faces)
        {
            face.Draw(
                viewVertices,
                worldVertices,
                projectedVertices,
                BaseShapeColor,
                camera,
                lightSource);
        }
    }
}
