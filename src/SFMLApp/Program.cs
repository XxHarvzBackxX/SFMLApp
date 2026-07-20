global using Color = SFML.Graphics.Color;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Shapes.LandscapeFeatures;
using SFMLApp.Utility;
using SFMLApp.Shapes;

namespace SFMLApp;

internal class Program
{
    private const string WindowTitle = "SFML App";
    private const uint WindowWidth = 1024;
    private const uint WindowHeight = 768;
    private const uint FramerateLimit = 60;
    private const Styles WindowStyle = Styles.Resize | Styles.Close;

    private static readonly ContextSettings GraphicsContext = new(
        depthBits: 0,
        stencilBits: 0,
        antialiasingLevel: 8,
        majorVersion: 1,
        minorVersion: 1,
        attributes: ContextSettings.Attribute.Default,
        sRgbCapable: false);

    public static RenderWindow? Window { get; private set; }
    public static readonly VideoMode VideoMode = new(WindowWidth, WindowHeight);

    public static Camera Camera = null!;
    public static Scene Scene = null!;

    public static bool DebugView { get; private set; }

    private const float _movementSpeed = 2.5f;
    private const float _rotationSpeed = 0.01f;
    private static bool _isRightMouseDragging;
    private static Vector2i _lastMousePosition;
    private const float _mouseSensitivity = 0.005f;

    private static void Main()
    {
        Window = CreateWindow();

        Camera = new Camera
        {
            Position = new Vector3f(0f, -3f, 15f),
            Rotation = new Vector2f(0f, 0f),
            Projection = new Projection()
        };

        LightSource redLight  = new(2f, -4f, -10f, 0f, 0f, 0f, 0.5f, Color.Red);
        LightSource redLight2 = new(5f, -3f, 6f, 0f, 0f, 0f, 0.5f, Color.Red) { Intensity = 10.0f };
        LightSource blueLight = new(-2f, -2f,  -8f, 0f, 0f, 0f, 0.5f, Color.Blue);
        LightSource blueLight2 = new(0, -3f, 2f, 0f, 0f, 0f, 0.5f, Color.Blue) { Intensity = 10.0f };
        LightSource greenLight = new(8f, 0f, 0f, 0f, 0f, 0f, 0.5f, Color.Green);
        LightSource whiteLight = new(0f, -8f, 6f, 4f, 0f, 0f, 1f, Color.White) { Intensity = 60.0f };

        Plane scenePlane = new(50, 1, 0f, new Terrain(new Vector3f(0f, -9f, 0f), 5f), new Terrain(new Vector3f(4f, 5f, 0f), 4f), new Terrain(new Vector3f(-6f, -5f, 4f), 6f));
        scenePlane.Position.Y += 6f;

        Cube cube = new(-2f, -2f, -10f);
        Sphere sphere = new(0f, -3f, 7f, layers: 32, slices: 64, baseShapeColor: new Color(200, 200, 200));
        Torus onionRing = new(4f, -3f, 2f, rotX: 0.45f, rotY: 0.2f, radius: 1.75f, thickness: 0.45f, segments: 64, tubeSegments: 20, baseShapeColor: new Color(220, 160, 80));
        ObjModel teapot = new(
            Path.Combine(AppContext.BaseDirectory, "assets", "utah_teapot.obj"),
            posX: 10f,
            posY: -1.5f,
            rotZ: MathF.PI,
            scale: 1.5f,
            baseShapeColor: new Color(210, 210, 220));
        ObjModel car = new(
            Path.Combine(AppContext.BaseDirectory, "assets", "car.obj"),
            posX: -10f,
            posY: 0f,
            rotZ: MathF.PI,
            scale: 0.003f,
            baseShapeColor: new Color(210, 210, 220));

        Orbit sphereOrbit = new(sphere, whiteLight)
        {
            Axis = OrbitAxis.X,
            Radius = 7.5f,
            AngularSpeed = 0.5f
        };

        Orbit redLightOrbit = new(redLight2, sphere)
        {
            Axis = OrbitAxis.Y,
            Radius = 5f,
            AngularSpeed = 0.7f,
            PhaseOffset = 0f,
            VerticalOscillationAmplitude = 2f,
            VerticalOscillationSpeed = 1.5f,
        };

        Orbit blueLightOrbit = new(blueLight2, sphere)
        {
            Axis = OrbitAxis.Y,
            Radius = 5f,
            AngularSpeed = 0.7f * 1.08f,
            PhaseOffset = MathF.PI,
            VerticalOscillationAmplitude = 0f
        };

        Scene = new Scene(
            lightSources: [redLight, blueLight, whiteLight, /*greenLight,*/ redLight2, blueLight2],
            objects:      [scenePlane, cube, sphere, onionRing, teapot, car],
            updatables:   [sphereOrbit, redLightOrbit, blueLightOrbit]);

        Clock clock = new();

        while (Window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();

            ProcessEvents();
            Update(cube, deltaTime);
            Render();
        }
    }

    private static void ProcessEvents()
    {
        Window!.DispatchEvents();
    }

    private static void Update(Cube cube, float deltaTime)
    {
        HandleInput(cube, deltaTime);
        Scene.Update(deltaTime); // physics? collisions? animations?

    }

    private static void Render()
    {
        Window!.Clear();
        Scene.Render(Camera);
        Window.Display();
    }

    private static RenderWindow CreateWindow()
    {
        RenderWindow window = new(VideoMode, WindowTitle, WindowStyle, GraphicsContext);

        window.SetFramerateLimit(FramerateLimit);
        window.SetKeyRepeatEnabled(false);

        window.Closed += (_, __) => window.Close();

        window.Resized += (_, args) =>
        {
            window.SetView(new View(new FloatRect(0, 0, args.Width, args.Height)));
        };

        window.KeyPressed += (_, e) =>
        {
            if (e.Code == Keyboard.Key.Delete)
                DebugView = !DebugView;
        };

        window.MouseButtonPressed += (_, e) =>
        {
            if (e.Button == Mouse.Button.Right)
            {
                _isRightMouseDragging = true;
                _lastMousePosition = Mouse.GetPosition(window);
            }
        };

        window.MouseButtonReleased += (_, e) =>
        {
            if (e.Button == Mouse.Button.Right)
                _isRightMouseDragging = false;
        };

        window.MouseMoved += HandleMouseCameraRotation;

        return window;
    }

    private static void HandleMouseCameraRotation(object? sender, MouseMoveEventArgs e)
    {
        if (!_isRightMouseDragging)
            return;

        Vector2i currentMousePosition = new(e.X, e.Y);

        Vector2i delta = currentMousePosition - _lastMousePosition;

        Camera.Rotation.Y += delta.X * _mouseSensitivity;
        Camera.Rotation.X += delta.Y * _mouseSensitivity;
        Camera.Rotation.X = Math.Clamp(Camera.Rotation.X, -1.5f, 1.5f);

        _lastMousePosition = currentMousePosition;
    }

    private static void HandleInput(Cube cube, float deltaTime)
    {
        float movement = _movementSpeed * deltaTime;

        HandleCameraMovement(movement);
        HandleCubeMovement(cube, movement);
        HandleCubeRotation(cube);

        
        // kebab / adele / tumble dryer

        // cube.Rotation = new Vector3f(cube.Rotation.X, cube.Rotation.Y + RotationSpeed, cube.Rotation.Z); 
        // kebab

        // cube.Rotation = new Vector3f(cube.Rotation.X + RotationSpeed, cube.Rotation.Y, cube.Rotation.Z); 
        // adele

        // cube.Rotation = new Vector3f(cube.Rotation.X, cube.Rotation.Y, cube.Rotation.Z + RotationSpeed); 
        // tumble dryer

        // cube.Rotation = new Vector3f(
        //     cube.Rotation.X + RotationSpeed,
        //     cube.Rotation.Y + RotationSpeed,
        //     cube.Rotation.Z + RotationSpeed);
        // kebab in Adele's tumble dryer
    }

    private static void HandleCameraMovement(float speed)
    {
        // lateral cam movement: WASD / vertical cam movement: QE

        Vector3f movement = new(0f, 0f, 0f);

        if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            movement += Camera.ForwardFlat;

        if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            movement -= Camera.ForwardFlat;

        if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            movement -= Camera.Right;

        if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            movement += Camera.Right;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            movement += Camera.Up;

        if (Keyboard.IsKeyPressed(Keyboard.Key.E))
            movement -= Camera.Up;

        if (Util.Magnitude(movement) > 0f)
            Camera.Position += Util.Normalize(movement) * speed;
    }

    private static void HandleCubeMovement(Cube cube, float speed)
    {
        // arrow keys for X/Z translation, V/B for Y translation

        if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            cube.Position.Z -= speed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            cube.Position.Z += speed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            cube.Position.X -= speed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            cube.Position.X += speed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.V))
            cube.Position.Y += speed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.B))
            cube.Position.Y -= speed;
    }

    private static void HandleCubeRotation(Cube cube)
    {
        // roll: R/F, pitch: T/G, yaw: Y/H

        if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            cube.Rotation.Z += _rotationSpeed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.F))
            cube.Rotation.Z -= _rotationSpeed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.T))
            cube.Rotation.X += _rotationSpeed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.G))
            cube.Rotation.X -= _rotationSpeed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Y))
            cube.Rotation.Y += _rotationSpeed;

        if (Keyboard.IsKeyPressed(Keyboard.Key.H))
            cube.Rotation.Y -= _rotationSpeed;
    }
}
