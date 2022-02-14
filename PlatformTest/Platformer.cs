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
        private Map map;
        private Matrix globalTransformation;
        private Camera camera;
        private float fps;
        SpriteFont font;
        private Goomba goomba1;
        private Player player;

        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            camera = new Camera(this, 0, 0);
            map = new Map(camera);
            player = new Player(map, camera);
            goomba1 = new Goomba(map, camera);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            //graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            map.Initialize(Content.RootDirectory);

            globalTransformation = Matrix.CreateScale((float)pixels);

            EntityManager.Add(player);
            EntityManager.Add(goomba1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map.Load(Services);
            player.Load(Services);
            font = Content.Load<SpriteFont>("Arial");
            goomba1.Load(Services);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

            map.Update(gameTime);

            //goomba1.Update(gameTime);

            //player.Update(gameTime);
            EntityManager.Update(gameTime);

            camera.CenterOnPlayer(player);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null,
                 globalTransformation);
            map.Draw(spriteBatch);
            EntityManager.Draw(spriteBatch);
            //goomba1.Draw(spriteBatch);
            //player.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "FPS:" + fps.ToString("00.00"), new Vector2(20, 20), Color.Red);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
