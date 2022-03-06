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
        private Matrix globalTransformation;
        private float fps;
        SpriteFont font;
        public GraphicsDevice gfx { get { return GraphicsDevice; } }

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
            //EntityManager.Add(new Goomba(new Vector2(13 * 16, 12 * 16)));
            //EntityManager.Add(new KoopaTrooper(new Vector2(31 * 16, 11 * 16)));
            //EntityManager.Add(new Goomba(new Vector2(42 * 16, 12 * 16)));
            //EntityManager.Add(new Goomba(new Vector2(43 * 16 + 8, 12 * 16)));
            //EntityManager.Add(new Goomba(new Vector2(75 * 16, 3 * 16)));
            //EntityManager.Add(new Goomba(new Vector2(77 * 16, 3 * 16)));
            //EntityManager.Add(new Goomba(new Vector2(89 * 16, 12 * 16)));
            //EntityManager.Add(new Goomba(new Vector2(90 * 16 + 8, 12 * 16)));
            //EntityManager.Add(new KoopaTrooper(new Vector2(98 * 16, 11 * 16)));

            //EntityManager.Init();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

            world.Update(gameTime);

            EntityManager.Update(gameTime);

            Camera.Instance.CenterOnPlayer();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null,
                 globalTransformation);
            EntityManager.DrawBehind(spriteBatch);
            world.Draw(spriteBatch);
            EntityManager.Draw(spriteBatch);

            spriteBatch.End();

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
