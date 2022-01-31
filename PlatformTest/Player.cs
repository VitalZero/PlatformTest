using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Player
    {
        private Texture2D texture;
        private Vector2 pos;
        private Vector2 vel;
        private float dir;
        private const float speed = 100f;
        private const float gravity = 20f;
        private const float jumpSpeed = -400f;
        KeyboardState keyState;
        ContentManager content;
        private Vector2 size;
        private bool isOnGround;
        private bool isJumping = false;

        public Player()
        {
            pos = new Vector2(50f, 50f);
            vel = Vector2.Zero;
            dir = 0f;
            size = new Vector2(14f, 30f);

        }

        public void Load(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            texture = content.Load<Texture2D>("mariobasic");
        }

        public void Input(GameTime gameTime)
        {
            KeyboardState oldState = keyState;
            keyState = Keyboard.GetState();
            
            if(keyState.IsKeyDown(Keys.Right))
                dir = 1f;
            if (keyState.IsKeyDown(Keys.Left))
                dir = -1f;

            //isJumping = keyState.IsKeyDown(Keys.Space);
            if (keyState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space))
                isJumping = true;
        }

        public void Update(GameTime gameTime, Map map)
        {
            ApplyPhysics(gameTime, map);

            dir = 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                pos,
                new Rectangle(0, 0, 16, 32),
                Color.White
                );
        }

        private void ApplyPhysics(GameTime gameTime, Map map)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 prevPos = pos;

            vel.X = dir * speed * elapsed;
            vel.Y = MathHelper.Clamp(vel.Y + gravity * elapsed, -10f, 10f);

            if (isJumping && isOnGround)
            {
                vel.Y = jumpSpeed * elapsed;
                isJumping = false;
            }

            pos += vel;
            pos = new Vector2((float)Math.Round(pos.X), (float)Math.Round(pos.Y));

            HandleCollisions(map);
        }

        private void HandleCollisions(Map map)
        {
            Rectangle bounds = new Rectangle((int)pos.X + 1, (int)pos.Y + 1, (int)size.X, (int)size.Y);
            isOnGround = false;

            int top = (int)Math.Floor((float)bounds.Top / 16);
            int bottom = (int)Math.Ceiling((float)bounds.Bottom / 16);
            int left = (int)Math.Floor((float)bounds.Left / 16);
            int right = (int)Math.Ceiling((float)bounds.Right / 16);

            //List<Tile> tilesToCheck = new List<Tile>();

            //collision handling
            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    //tilesToCheck.Add(map.GetTile(x, y));
                    Tile tile = map.GetTile(x, y);

                    bounds = new Rectangle((int)pos.X + 1, (int)pos.Y + 1, (int)size.X, (int)size.Y);

                    if (tile.collision != TileCollision.none)
                    {
                        Rectangle tileBounds = map.GetBounds(x, y);

                        Vector2 depth = GetIntersectionDepth(bounds, tileBounds);

                        if (depth != Vector2.Zero)
                        {
                            float depthX = Math.Abs(depth.X);
                            float depthY = Math.Abs(depth.Y);

                            // check first X and resolve
                            if (depthX < depthY)
                            {
                                pos.X += depth.X;
                                bounds.X = (int)pos.X + 1;
                                vel.X = 0;
                            }
                            else
                            {
                                if(tile.collision == TileCollision.breakable &&
                                    vel.Y < 0)
                                {
                                    map.RemoveTile(x, y);

                                    // if tile is breakable (brick) reduce Y speed by 70% of the current speed
                                    vel.Y += -vel.Y * .7f ;
                                }
                                else
                                {
                                    vel.Y = 0;
                                }

                                pos.Y += depth.Y;
                                isOnGround = true;
                                bounds.Y = (int)pos.Y + 1;
                            }
                        }
                    }
                }
            }

            //foreach(var t in tilesToCheck)
            //{
            //    if (t.collision != TileCollision.none)
            //    {
            //        bounds = new Rectangle((int)pos.X + 1, (int)pos.Y + 1, (int)size.X, (int)size.Y);

            //        Rectangle tileBounds = new Rectangle(t.X * 16, t.Y * 16, 16, 16);

            //        Vector2 depth = GetIntersectionDepth(bounds, tileBounds);

            //        if (depth != Vector2.Zero)
            //        {
            //            float depthX = Math.Abs(depth.X);
            //            float depthY = Math.Abs(depth.Y);

            //            // check first X and resolve
            //            if (depthX < depthY)
            //            {
            //                pos.X += depth.X;
            //                bounds.X = (int)pos.X + 1;
            //                vel.X = 0;
            //            }
            //            else
            //            {
            //                pos.Y += depth.Y;
            //                isOnGround = true;
            //                bounds.Y = (int)pos.Y + 1;
            //                vel.Y = 0;
            //            }
            //        }
            //    }
            //}
        }

        private Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) > minDistanceX || Math.Abs(distanceY) > minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}
