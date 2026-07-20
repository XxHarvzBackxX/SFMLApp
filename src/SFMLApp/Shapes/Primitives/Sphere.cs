using SFML.System;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;
using SFMLApp.Utility;

namespace SFMLApp.Shapes.Primitives
{
    public class Sphere : Shape3D
    {
        public int Layers { get; }
        public int Slices { get; }

        public Sphere(
            float posX = 0f,
            float posY = 0f,
            float posZ = 0f,
            float rotX = 0f,
            float rotY = 0f,
            float rotZ = 0f,
            float scale = 1f,
            int layers = 16,
            int slices = 32,
            Color? baseShapeColor = null)
        {
            Position = new Vector3f(posX, posY, posZ);
            Rotation = new Vector3f(rotX, rotY, rotZ);
            Scale = scale;
            BaseShapeColor = baseShapeColor ?? new Color(255, 255, 255);
            Layers = layers;
            Slices = slices;

            _initModels();
        }

        private void _initModels()
        {
            List<float[]> vertices = [[1f, 0f, 0f]];

            for (int layer = 1; layer <= Layers; layer++)
            {
                float polarAngle = layer * MathF.PI / (Layers + 1);
                float x = MathF.Cos(polarAngle);
                float ringRadius = MathF.Sin(polarAngle);

                for (int slice = 0; slice < Slices; slice++)
                {
                    float azimuth = slice * 2f * MathF.PI / Slices;
                    vertices.Add([
                        x,
                        ringRadius * MathF.Cos(azimuth),
                        ringRadius * MathF.Sin(azimuth)
                    ]);
                }
            }

            int westPoleIndex = vertices.Count;
            vertices.Add([-1f, 0f, 0f]);
            _model = vertices.ToArray();

            List<Face> faces = [];

            for (int slice = 0; slice < Slices; slice++)
            {
                int current = 1 + slice;
                int next = 1 + (slice + 1) % Slices;
                faces.Add(new Face([0, current, next, 0], this));
            }

            for (int layer = 0; layer < Layers - 1; layer++)
            {
                int currentRing = 1 + layer * Slices;
                int nextRing = currentRing + Slices;

                for (int slice = 0; slice < Slices; slice++)
                {
                    int nextSlice = (slice + 1) % Slices;
                    faces.Add(new Face([
                        currentRing + slice,
                        nextRing + slice,
                        nextRing + nextSlice,
                        currentRing + nextSlice
                    ], this));
                }
            }

            int lastRing = 1 + (Layers - 1) * Slices;
            for (int slice = 0; slice < Slices; slice++)
            {
                int current = lastRing + slice;
                int next = lastRing + (slice + 1) % Slices;
                faces.Add(new Face([current, westPoleIndex, next, current], this));
            }

            Faces = faces.ToArray();
        }

        public override Vector3f GetVertexNormal(int vertexIndex, Vector3f worldPoint, Vector3f faceNormal) =>
            Util.Normalize(worldPoint - Position);

        public override IEnumerable<DrawCall> CollectFaces(Camera camera, IReadOnlyList<LightSource> lightSources)
        {
            Vector3f[] worldVertices = new Vector3f[_model.Length];
            Vector3f[] viewVertices = new Vector3f[_model.Length];
            bool[] verticesInView = new bool[_model.Length];

            for (int vertexIndex = 0; vertexIndex < _model.Length; vertexIndex++)
            {
                float[] vertexCoords = _model[vertexIndex];

                Vector3f modelSpacePoint = new(vertexCoords[0], vertexCoords[1], vertexCoords[2]);
                Vector3f localSpacePoint = Util.ToLocal(modelSpacePoint, Scale, Rotation);

                worldVertices[vertexIndex] = Util.ToWorld(localSpacePoint, Position);
                (viewVertices[vertexIndex], verticesInView[vertexIndex]) =
                    Util.ToView(worldVertices[vertexIndex], camera);
            }

            // only project vertices inside the camera frustum to avoid crap
            Vector2f[] projectedVertices = new Vector2f[_model.Length];
            for (int vertexIndex = 0; vertexIndex < _model.Length; vertexIndex++)
            {
                if (verticesInView[vertexIndex])
                    projectedVertices[vertexIndex] = Util.ToXY(viewVertices[vertexIndex]);
            }

            foreach (Face face in Faces)
            {
                DrawCall? drawCall = face.TryGetDrawCall(
                    viewVertices,
                    worldVertices,
                    projectedVertices,
                    verticesInView,
                    BaseShapeColor,
                    camera,
                    lightSources);

                if (drawCall is not null)
                    yield return drawCall.Value;
            }
        }
    }
}
