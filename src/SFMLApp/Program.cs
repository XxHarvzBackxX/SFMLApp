using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFMLApp.Infrastructure;
using SFMLApp.Shapes.Base;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Utility;

namespace SFMLApp;

internal class Program
{
    public static RenderWindow? Window { get; private set; }
    public static readonly VideoMode VideoMode = new(800, 600);

    public static Camera Camera = null!;
    public static LightSource LightSource = null!;

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
            Position = new Vector3f(0f, 0f, 0f),
            Rotation = new Vector2f(0f, 0f)
        };

        Cube cube = new(-2f, 2f, -10f, 0f, 0f, 0f, 1f);
        LightSource = new LightSource(2f, -2f, -10f, 0f, 0f, 0f, 1f, Color.Blue);

        Clock clock = new();

        while (Window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();

            ProcessEvents();
            Update(cube, deltaTime);
            Render(cube);
        }
    }

    private static void ProcessEvents()
    {
        Window!.DispatchEvents();
    }

    private static void Update(Cube cube, float deltaTime)
    {
        HandleInput(cube, deltaTime);

        // physics?
        // collisions?
        // animations?
    }

    private static void Render(SimpleShape shape)
    {
        Window!.Clear();

        if (shape is Cube cube)
            cube.Draw(Camera, LightSource);

        LightSource.Draw(Camera, LightSource);

        Window.Display();
    }

    private static RenderWindow CreateWindow()
    {
        RenderWindow window = new(VideoMode, "SFML Window", Styles.Resize | Styles.Close);

        window.SetFramerateLimit(60);
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

        window.MouseMoved += (_, e) =>
        {
            if (!_isRightMouseDragging)
                return;

            Vector2i currentMousePosition = new(e.X, e.Y);

            Vector2i delta = currentMousePosition - _lastMousePosition;

            Camera.Rotation.Y += delta.X * _mouseSensitivity;
            Camera.Rotation.X += delta.Y * _mouseSensitivity;
            Camera.Rotation.X = Math.Clamp(Camera.Rotation.X, -1.5f, 1.5f);

            _lastMousePosition = currentMousePosition;
        };

        return window;
    }

    private static void HandleInput(Cube cube, float deltaTime)
    {
        float movement = _movementSpeed * deltaTime;

        HandleCameraMovement(movement);
        HandleCubeMovement(cube, movement);
        HandleCubeRotation(cube);

        
        // KEBAB / ADELE / TUMBLE DRYER

        // cube.Rotation = new Vector3f(cube.Rotation.X, cube.Rotation.Y + RotationSpeed, cube.Rotation.Z); 
        // kebab

        // cube.Rotation = new Vector3f(cube.Rotation.X + RotationSpeed, cube.Rotation.Y, cube.Rotation.Z); 
        // Adele

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
        // WASD lateral cam movement / QE for vertical cam movement

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
        // R/F = roll, T/G = pitch, Y/H = yaw

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