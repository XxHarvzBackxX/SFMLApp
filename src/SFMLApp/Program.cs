using SFML.Window;
using SFML.Graphics;
using SFML.System;
using SFMLApp.Shapes.Primitives;

namespace SFMLApp;

internal class Program
{
    public static RenderWindow? Window { get; private set; }
    public static readonly VideoMode VideoMode = new VideoMode(800, 600);
    static void Main(string[] args)
    {
        Window = new RenderWindow(VideoMode, "SFML Window", Styles.Resize | Styles.Close);
        Window.SetFramerateLimit(60);
        Window.SetKeyRepeatEnabled(false);

        //CircleShape shape = Util.CircleFactory(20f, Color.Red, new Vector2f(0f, 0f));
        //Vector2f vel = new Vector2f(150f, 150f);

        //RectangleShape line = Util.SolidLineFactory(new Vector2f(200f, 200f), new Vector2f(400f, 400f), 5f, Color.Blue);
        //VertexArray gradientLine = Util.GradientLineFactory(new Vector2f(200f, 200f), new Vector2f(400f, 400f), 5f, Color.Blue, Color.Red);
        Cube cube = new Cube(-2f, -2f, -10f, 0f, 0f, 0f, 1f);

        Clock clock = new Clock();

        Window.Closed += (sender, args) => Window.Close();
        Window.Resized += (sender, args) => Window.SetView(new View(new FloatRect(0, 0, args.Width, args.Height)));

        while (Window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();
            Window.DispatchEvents();
            Window.Clear();

            //shape.Position += vel * deltaTime;
            //if (shape.Position.Y + (shape.Radius * 2) > Window.Size.Y)
            //    vel.Y *= -1;
            //Window.Draw(line);
            //Window.Draw(gradientLine);
            //Window.Draw(shape);

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

            cube.Draw();
            Window.Display();
        }
    }
}
