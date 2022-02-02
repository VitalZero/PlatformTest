using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        private const int width = 480;
        private const int height = 240;
        private const int pixels = 2;
        private const int windowWidth = width * pixels;
        private const int windowHeight = height * pixels;
        private RenderTarget2D buffer;
        private Map map;
        private Player player;
        private Matrix globalTransformation;
        private Camera camera;
        private float fps;
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            camera = new Camera(this, 0, 0);
            map = new Map(camera);
            player = new Player(camera);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            buffer = new RenderTarget2D(GraphicsDevice, width, height);
            map.Initialize(Content.RootDirectory);

            globalTransformation = Matrix.CreateScale((float)pixels);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map.Load(Services);
            player.Load(Services);
            font = Content.Load<SpriteFont>("Arial");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Input(gameTime);

            player.Update(gameTime, map);

            camera.CenterOnPlayer(player);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, globalTransformation);
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);

            spriteBatch.DrawString(font, "FPS:" + fps.ToString("00.00"), new Vector2(20, 20), Color.Red);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
