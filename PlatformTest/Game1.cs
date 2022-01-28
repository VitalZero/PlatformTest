using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private const int width = 480;
        private const int height = 240;
        private const int pixels = 3;
        private const int windowWidth = width * pixels;
        private const int windowHeight = height * pixels;
        private RenderTarget2D buffer;
        private Map map;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            map = new Map();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            buffer = new RenderTarget2D(GraphicsDevice, width, height);
            map.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map.Load(Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(buffer);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            map.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            spriteBatch.Draw(buffer, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
