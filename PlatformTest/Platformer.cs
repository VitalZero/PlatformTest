using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformTest
{
    public class Platformer : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        private const int width = 320;
        private const int height = 240;
        private const int pixels = 3;
        private const int windowWidth = width * pixels;
        private const int windowHeight = height * pixels;
        private World world;
        private KeyboardState keyboard;
        private Matrix globalTransformation;
        private float fps;
        SpriteFont font;
        public GraphicsDevice gfx { get { return GraphicsDevice; } }
        private RenderTarget2D renderTarget;
        private bool pause = false;
        private bool advance = false;

        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            world = new World();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            //graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            Camera.Instance.Init(this, 0, 0);

            world.Initialize(Content.RootDirectory);

            globalTransformation = Matrix.CreateScale((float)pixels);

            renderTarget = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ResourceManager.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            world.Load();
            font = ResourceManager.Arial;
            // TODO: use this.Content to load your game content here

            EntityManager.Add(Player.Instance);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (keyboard.IsKeyDown(Keys.W) && oldState.IsKeyUp(Keys.W))
                pause = !pause;
            if (keyboard.IsKeyDown(Keys.Q) && oldState.IsKeyUp(Keys.Q))
                advance = true;

            fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!pause || ( advance))
            {
                world.Update(gameTime);

                EntityManager.Update(gameTime);

                Camera.Instance.CenterOnPlayer();
            }

            advance = false;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // draw to a texture
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            EntityManager.DrawBehind(spriteBatch);
            world.Draw(spriteBatch);
            EntityManager.Draw(spriteBatch);

            spriteBatch.End();

            // then draw to the screen 
            // so we can apply the scale to size of the window (globalTransformation) without errors
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null,
                 globalTransformation);
            spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            // draw info stuff
            spriteBatch.Begin();
            spriteBatch.DrawString(ResourceManager.Arial, "FPS:" + fps.ToString("00.00"), new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(ResourceManager.Arial, "vel X:" + Player.Instance.Vel.X.ToString("00.0000"), new Vector2(20, 40), Color.Red);
            spriteBatch.DrawString(ResourceManager.Arial, "vel Y:" + Player.Instance.Vel.Y.ToString("00.0000"), new Vector2(20, 60), Color.Red);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
