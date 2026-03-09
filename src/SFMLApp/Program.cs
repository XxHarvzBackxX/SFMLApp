using SFML.Window;
using SFML.Graphics;
using SFML.System;

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
        Cube cube = new Cube(2f, 2f, -10f, 0f, 0f, 0f, 1f);

        Clock clock = new Clock();

        Window.Closed += (sender, args) => Window.Close();

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
            Vector3f pos = cube.Position;

            // wasd for X and Z translation
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                pos.Z -= speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                pos.Z += speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                pos.X += speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                pos.X -= speed;

            // Q and E for Y translation
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                pos.Y -= speed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                pos.Y += speed;

            cube.Position = pos;

            cube.Draw();
            Window.Display();
        }
    }
}
