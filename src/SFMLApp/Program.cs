using SFML.Window;
using SFML.Graphics;
using SFML.System;
using SFMLApp.Shapes.Primitives;
using SFMLApp.Infrastructure;

namespace SFMLApp;

internal class Program
{
    public static RenderWindow? Window { get; private set; }
    public static readonly VideoMode VideoMode = new VideoMode(800, 600);
    public static Camera Camera = null!;
    static void Main(string[] args)
    {
        Window = new RenderWindow(VideoMode, "SFML Window", Styles.Resize | Styles.Close);
        Window.SetFramerateLimit(60);
        Window.SetKeyRepeatEnabled(false);

        Camera = new Camera()
        {
            Position = new Vector3f(0, 0, 0),
        };
        

        Cube cube = new Cube(-2f, -2f, -10f, 0f, 0f, 0f, 1f);

        Clock clock = new Clock();

        Window.Closed += (sender, args) => Window.Close();
        Window.Resized += (sender, args) => Window.SetView(new View(new FloatRect(0, 0, args.Width, args.Height)));

        while (Window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();
            Window.DispatchEvents();
            Window.Clear();

            float speed = 2.5f * deltaTime;

            // wasd for X and Z translation
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                cube.Position.Z -= speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                cube.Position.Z += speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                cube.Position.X -= speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                cube.Position.X += speed;

            // Q and E for Y translation
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                cube.Position.Y += speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                cube.Position.Y -= speed;

            // R and F for roll
            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                cube.Rotation.Z += 0.01f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.F))
                cube.Rotation.Z -= 0.01f;

            // T and G for pitch
            if (Keyboard.IsKeyPressed(Keyboard.Key.T))
                cube.Rotation.X += 0.01f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.G))
                cube.Rotation.X -= 0.01f;

            // Y and H for yaw
            if (Keyboard.IsKeyPressed(Keyboard.Key.Y))
                cube.Rotation.Y += 0.01f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.H))
                cube.Rotation.Y -= 0.01f;

            //cube.Rotation = new Vector3f(cube.Rotation.X, cube.Rotation.Y + 0.01f, cube.Rotation.Z); // kebab
            //cube.Rotation = new Vector3f(cube.Rotation.X + 0.01f, cube.Rotation.Y, cube.Rotation.Z); // adele
            //cube.Rotation = new Vector3f(cube.Rotation.X, cube.Rotation.Y, cube.Rotation.Z + 0.01f); // tumble dryer
            //cube.Rotation = new Vector3f(cube.Rotation.X + 0.01f, cube.Rotation.Y + 0.01f, cube.Rotation.Z + 0.01f); // kebab in adele's tumble dryer

            cube.Draw(Camera);
            Window.Display();
        }
    }
}
