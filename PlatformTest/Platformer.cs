using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;
using VZShapes;

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
        public Shapes shapes;
        Camera2D cam;
        //Stopwatch stopWatch;


        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            world = new World();
            cam = new Camera2D(new Rectangle(0, 0, width, height), new Rectangle(0, 0, windowWidth, windowHeight));
            //stopWatch = new Stopwatch();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferMultiSampling = false;
            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //TargetElapsedTime = TimeSpan.FromSeconds(1f / 120f);
            graphics.ApplyChanges();

            world.Initialize(Content.RootDirectory);

            globalTransformation = Matrix.CreateScale((float)pixels);

            renderTarget = new RenderTarget2D(GraphicsDevice, width, height);

            shapes = new Shapes(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextureManager.Load(Content);
            SoundManager.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            world.Load();
            font = TextureManager.Arial;
            // TODO: use this.Content to load your game content here

            EntityManager.Add(Player.Instance);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(SoundManager.SurfaceStage);

            Player.Instance.deathEvent += StopMusic;
        }

        protected override void UnloadContent()
        {
            Player.Instance.deathEvent -= StopMusic;

            base.UnloadContent();
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
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

            if (!pause || advance)
            {
                if (!Player.Instance.IsTransforming)
                {
                    //stopWatch.Reset();
                    //stopWatch.Start();

                    world.Update(gameTime);

                    SpriteManager.Update(gameTime);

                    EntityManager.Update(gameTime);

                    //stopWatch.Stop();
                }
                else
                {
                    Player.Instance.Update(gameTime);
                }
                
                Camera2D.Instance.Follow(Player.Instance, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            //Debug.WriteLine(stopWatch.Elapsed.TotalSeconds.ToString("0.000000"));

            advance = false;
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Rectangle paabb = Player.Instance.GetAABB();
            // draw to a texture
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(World.Instance.bgColor);//(Color.CornflowerBlue);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera2D.Instance.Transform * Camera2D.Instance.CameraShake);

            EntityManager.DrawBehind(spriteBatch);

            world.Draw(spriteBatch);

            SpriteManager.Draw(spriteBatch);

            EntityManager.Draw(spriteBatch);
            //shapes.Begin();
            //shapes.DrawRectangle(paabb.X + Camera2D.Instance.Transform.Translation.X, paabb.Y, paabb.Width, paabb.Height, 1, new Color(Color.Indigo, 0.5f));
            //shapes.End();

            spriteBatch.End();


            // then draw to the screen 
            // so we can apply the scale to size of the window (globalTransformation) without errors
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: globalTransformation);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // draw info stuff

            Vector2 playerScreenPos = Camera2D.Instance.WorldToScreen(Player.Instance.Position);
            spriteBatch.Begin();
            //spriteBatch.DrawString(TextureManager.Arial, "FPS: " + fps.ToString("00.00"), new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "vel X: " + Player.Instance.Vel.X.ToString("00.0000"), new Vector2(20, 35), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "vel Y: " + Player.Instance.Vel.Y.ToString("00.0000"), new Vector2(20, 50), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "Sprites: " + SpriteManager.Count, new Vector2(20, 65), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "Entities total: " + EntityManager.Count, new Vector2(20, 80), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "Enemies total: " + EntityManager.EnemiesCount, new Vector2(20, 95), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "ScreenX: " + playerScreenPos.X.ToString("00.00"), new Vector2(20, 110), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "ScreenY: " + playerScreenPos.Y.ToString("00.00"), new Vector2(20, 125), Color.Red);
            spriteBatch.DrawString(TextureManager.Arial, "GameTime.DT: " + gameTime.ElapsedGameTime.TotalSeconds.ToString("0.00000"), new Vector2(20, 135), Color.Red);
            spriteBatch.End();

            this.Window.Title = fps.ToString("00.00");

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
