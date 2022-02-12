//#define DEBUG_DRAW

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace PlatformTest
{
    public class Player
    {
        private enum States { stand, run, jump, fall }

#if DEBUG_DRAW
        Texture2D debugTexture;
#endif
        public Vector2 Pos { get { return pos; } }
        public bool isOnGround { get; set; }

        private Texture2D texture;
        private Vector2 pos;
        private Vector2 vel;
        private Vector2 origin;
        private Vector2 size;
        private float dir;
        private const float speed = 100f;
        private const float gravity = 20f;
        private const float jumpSpeed = -400f;
        private KeyboardState keyboard;
        private ContentManager content;
        private bool isJumping;
        private Rectangle aabb;
        private Camera camera;
        private SpriteEffects flip;
        private Animation standing;
        private Animation run;
        private Animation jump;
        private Animation fall;
        private AnimationPlayer animPlayer;
        private Map map;
        States currState;
        public bool Pause { get; set; }

        //for debug purposes
        List<string> playerStates = new List<string>();
        SpriteFont font;
        //

        public Player(Camera camera, Map map)
        {
            pos = new Vector2(50f, 50f);
            size = new Vector2(16, 31);
            vel = Vector2.Zero;
            dir = 0f;
            aabb = new Rectangle(2, 4, 12, 27);
            origin = new Vector2(size.X / 2, size.Y);
            this.camera = camera;
            animPlayer = new AnimationPlayer();
            isOnGround = false;
            this.map = map;
            Pause = false;

            currState = States.fall;

            //for debug
            playerStates.Add("STAND");
            playerStates.Add("RUN");
            playerStates.Add("JUMP");
            playerStates.Add("FALL");
            //
        }

        public void Load(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

#if DEBUG_DRAW
            debugTexture = content.Load<Texture2D>("pixel");
#endif

            texture = content.Load<Texture2D>("mariobasic");

            standing = new Animation(texture, 1f, true, 16, 1, 0, 0);
            run = new Animation(texture, .1f, true, 16, 4, 16, 0);
            jump = new Animation(texture, .1f, true, 16, 1, 16 * 6, 0);
            fall = new Animation(texture, 1f, true, 16, 1, 16 * 5, 0);
            animPlayer.PlayAnimation(standing);
            font = content.Load<SpriteFont>("Arial");
        }

        public void Input(GameTime gameTime)
        {
            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            //isJumping = keyState.IsKeyDown(Keys.Space);
            if (keyboard.IsKeyDown(Keys.Space))
                isJumping = true;

            if (keyboard.IsKeyDown(Keys.Right))
            {
                dir = 1f;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            { 
                dir = -1f;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A))
                Pause = !Pause;

            if (Pause)
                return;

            vel.Y += gravity * elapsed;

            switch (currState)
            {
                case States.stand:
                    {
                        animPlayer.PlayAnimation(standing);
                        vel.X = 0;

                        if (!isOnGround)
                        {
                            currState = States.fall;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Right) != keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.run;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        {
                            currState = States.jump;
                            vel.Y = jumpSpeed * elapsed;
                            isOnGround = false;
                            break;
                        }

                    }
                    break;

                case States.run:
                    {
                        animPlayer.PlayAnimation(run);

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            vel.X = 0;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X = speed * elapsed;
                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = -speed * elapsed;
                            flip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        {
                            currState = States.jump;
                            vel.Y = jumpSpeed * elapsed;
                            isOnGround = false;
                            break;
                        }
                        else if (!isOnGround)
                        {
                            currState = States.fall;
                            break;
                        }
                    }
                    break;

                case States.jump:
                    {
                        animPlayer.PlayAnimation(jump);

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = 0f;
                        }
                        else if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X = speed * elapsed;
                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = -speed * elapsed;
                            flip = SpriteEffects.FlipHorizontally;
                        }

                        if(isOnGround)
                        {
                            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                            {
                                currState = States.stand;
                                vel = Vector2.Zero;
                                break;
                            }
                            else
                            {
                                currState = States.run;
                                vel.Y = 0f;
                                break;
                            }
                        }

                        if(vel.Y > 0)
                        {
                            currState = States.fall;
                            break;
                        }
                    }
                    break;

                case States.fall:

                    //animPlayer.PlayAnimation(fall);

                    if (isOnGround)
                    {
                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            vel = Vector2.Zero;
                        }
                        else
                        {
                            currState = States.run;
                            vel.Y = 0f;
                        }
                    }

                    break;
            }

            Physics(gameTime);

            //if (vel.X > 0)
            //{

            //    if (isOnGround)
            //    {
            //        animPlayer.PlayAnimation(run);
            //        flip = SpriteEffects.None;
            //    }
            //}
            //else if (vel.X < 0)
            //{
            //    if (isOnGround)
            //    {
            //        animPlayer.PlayAnimation(run);
            //        flip = SpriteEffects.FlipHorizontally;
            //    }
            //}
            //else
            //{
            //    if (isOnGround)
            //        animPlayer.PlayAnimation(standing);
            //}

            if(currState != States.fall)
                animPlayer.Update(gameTime);

            isJumping = false;

            dir = 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch, 
                new Vector2((int)pos.X - (int)camera.XOffset, (int)pos.Y - (int)camera.YOffset), 
                flip);

            spriteBatch.DrawString(font, playerStates[(int)currState], 
                new Vector2((int)pos.X - 10 - (int)camera.XOffset, (int)pos.Y - 20 - (int)camera.YOffset), Color.Crimson);

#if DEBUG_DRAW
            Rectangle aabbDebug = GetAABB();
            //aabbDebug.X += (int)-camera.XOffset;
            //aabbDebug.Y += (int)-camera.YOffset;
            //spriteBatch.Draw(debugTexture, aabbDebug, new Color(Color.Red, 0.2f));

            spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)camera.XOffset, ((aabbDebug.Top / 16) * 16) - (int)camera.YOffset, 16, 16),
                new Color(Color.White, 0.2f));
            spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)camera.XOffset, ((aabbDebug.Bottom / 16) * 16) - (int)camera.YOffset, 16, 16),
                new Color(Color.Green, 0.2f));

            spriteBatch.Draw(debugTexture, new Rectangle(
                (int)(pos.X + origin.X - camera.XOffset) - 1, 
                (int)(pos.Y + origin.Y - camera.YOffset) - 1,
                2, 2),
                new Color(Color.White, 0.8f));
#endif
        }

        public Rectangle GetAABB()
        {
            return new Rectangle((int)pos.X + aabb.X, (int)pos.Y + aabb.Y, aabb.Width, aabb.Height);
        }

        private void Physics(GameTime gameTime)
        {
            // was on physics before
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //vel.X = dir * speed * elapsed;
            //vel.Y = MathHelper.Clamp(vel.Y + gravity * elapsed, -10f, 10f);

            //if (isJumping && isOnGround)
            //{
            //    {
            //        animPlayer.PlayAnimation(jump);
            //        vel.Y = jumpSpeed * elapsed;
            //    }
            //}



            pos.X += vel.X;
            //pos.X = (float)Math.Round(pos.X);
            
            //HandleCollisionsX();
            HandlecollisionHorizontal();

            pos.Y += vel.Y;
            //pos.Y = (float)Math.Round(pos.Y);

            //isOnGround = false;

            HandlecollisionVertical();
            //HandleCollisionsY();
        }

        private void HandlecollisionHorizontal()
        {
            Rectangle bounds = GetAABB();

            int top = bounds.Top / 16;
            int bottom = bounds.Bottom / 16;
            int left = bounds.Left / 16;
            int right = bounds.Right / 16;

            // if we're going right, check all the tiles to the right, from top to bottom
            if (vel.X > 0)
            {

                List<Tile> tilesToCheck = new List<Tile>();

                for(int i = top; i <= bottom; ++i)
                {
                    tilesToCheck.Add(map.GetTile(right, i));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = map.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            float depth = bounds.Right - tileBounds.Left;

                            pos.X -= depth;
                            break;
                        }
                    }
                }
            }
            // if we're going left, check all the tiles to the left, from top to bottom
            else if (vel.X < 0)
            {
                List<Tile> tilesToCheck = new List<Tile>();

                for (int i = top; i <= bottom; ++i)
                {
                    tilesToCheck.Add(map.GetTile(left, i));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = map.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            float depth = bounds.Left - tileBounds.Right;

                            pos.X -= depth;
                            break;
                        }
                    }
                }
            }
        }

        private void HandlecollisionVertical()
        {
            Rectangle bounds = GetAABB();

            int top = bounds.Top / 16;
            int bottom = bounds.Bottom / 16;
            int left = bounds.Left / 16;
            int right = bounds.Right / 16;

            // if we're going down, check the bottom tiles from left to right
            if (vel.Y > 0)
            {
                List<Tile> tilesToCheck = new List<Tile>();
                
                for (int i = left; i <= right; ++i)
                {
                    tilesToCheck.Add(map.GetTile(i, bottom));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = map.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            float depth = bounds.Bottom - tileBounds.Top;

                            pos.Y -= depth;
                            isOnGround = true;
                            vel.Y = 0;
                            //break;
                        }
                    }
                    else
                    {
                        bounds = GetAABB();

                        if (bounds.Bottom > (t.Y * 16) + 1)
                        {
                            isOnGround = false;
                        }
                    }
                }
            }
            // if we're going up, check the top tiles from left to right
            else if (vel.Y < 0)
            {
                List<Tile> tilesToCheck = new List<Tile>();

                for (int i = left; i <= right; ++i)
                {
                    tilesToCheck.Add(map.GetTile(i, top));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = map.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            float depth = bounds.Top - tileBounds.Bottom;

                            if (t.collision == TileCollision.breakable)
                                map.RemoveTile(t.X, t.Y);
                            else if (t.collision == TileCollision.item)
                                map.usedTileItem(t.X, t.Y);

                            pos.Y -= depth;
                            vel.Y = 0;
                            break;
                        }
                    }
                }
            }
        }

        private void HandleCollisionsX()
        {
            Rectangle bounds = GetAABB();

            int top = (int)(bounds.Top / 16);
            int bottom = (int)((bounds.Bottom) / 16);
            int left = (int)(bounds.Left / 16);
            int right = (int)((bounds.Right) / 16);

            if (top < 0) top = 0;
            if (left < 0) left = 0;
            if (right >= map.mapWidth) right = map.mapWidth;
            if (bottom >= map.mapHeight) bottom = map.mapHeight;

            List<Tile> tilesToCheck = new List<Tile>();

            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    Tile tile = map.GetTile(x, y);

                    if (tile.collision != TileCollision.none)
                    {
                        Rectangle tileBounds = new Rectangle(tile.X * 16, tile.Y * 16, 16, 16);

                        if(bounds.Intersects(tileBounds))
                        {
                            float leftDist = Math.Abs(bounds.Right - tileBounds.Left);
                            float rightDist = Math.Abs(bounds.Left - tileBounds.Right);

                            if(leftDist < rightDist)
                            {
                                pos.X += -leftDist;
                            }
                            else
                            {
                                pos.X += rightDist;
                            }

                            return;
                        }
                    }
                }
            }
        }

        private void HandleCollisionsY()
        {
            Rectangle bounds = GetAABB();

            int top = (int)(bounds.Top / 16);
            int bottom = (int)((bounds.Bottom) / 16);
            int left = (int)(bounds.Left / 16);
            int right = (int)((bounds.Right) / 16);

            if (top < 0) top = 0;
            if (left < 0) left = 0;
            if (right >= map.mapWidth) right = map.mapWidth;
            if (bottom >= map.mapHeight) bottom = map.mapHeight;

            List<Tile> tilesToCheck = new List<Tile>();

            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    Tile tile = map.GetTile(x, y);

                    if (tile.collision != TileCollision.none)
                    {
                        Rectangle tileBounds = new Rectangle(tile.X * 16, tile.Y * 16, 16, 16);

                        if (bounds.Intersects(tileBounds))
                        {
                            float topDist = Math.Abs(bounds.Bottom - tileBounds.Top);
                            float bottomDist = Math.Abs(bounds.Top - tileBounds.Bottom);

                            if (topDist < bottomDist)
                            {
                                pos.Y += -topDist;
                                isOnGround = true;
                                vel.Y = 0;
                            }
                            else
                            {
                                if (tile.collision == TileCollision.breakable)
                                    map.RemoveTile(tile.X, tile.Y);
                                else if (tile.collision == TileCollision.item)
                                    map.usedTileItem(tile.X, tile.Y);

                                pos.Y += bottomDist;
                                vel.Y = 0;
                            }

                            return;
                        }
                    }
                }
            }
        }
    }
}
