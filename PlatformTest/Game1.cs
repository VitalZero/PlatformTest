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
        private Player player;
        private Matrix globalTransformation;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            map = new Map();
            player = new Player();
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
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Input(gameTime);

            player.Update(gameTime, map);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.SetRenderTarget(buffer);

            //GraphicsDevice.Clear(Color.CornflowerBlue);
            //spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //map.Draw(spriteBatch);
            //player.Draw(spriteBatch);

            //spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(null);

            //spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            //spriteBatch.Draw(buffer, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
            //spriteBatch.End();

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, globalTransformation);
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
