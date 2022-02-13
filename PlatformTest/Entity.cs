using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Entity : GameObject
    {
        protected Map map;
        protected Camera camera;
        protected Vector2 vel;
        protected float speed = 150f;
        protected float gravity = 20f;
        protected float jumpSpeed = -400f;
        protected bool isOnGround;
        protected float elapsed;

        public Vector2 Pos { get { return pos; } }

        public Entity(Map map, Camera camera)
        {
            this.map = map;
            this.camera = camera;
            isOnGround = false;
            elapsed = 0;
        }

        public void Init(float speed, float gravity, float jumpSpeed)
        {
            this.speed = speed;
            this.gravity = gravity;
            this.jumpSpeed = jumpSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, 
                new Vector2((int)pos.X - (int)camera.XOffset, (int)pos.Y - (int)camera.YOffset),
                Color.White);
        }

        public override void Update(GameTime gameTime)
        {
        }

        protected void LateUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsed = dt / 2;

            while (dt > 0.0f)
            {
                Physics();
                dt -= elapsed;
            }
        }

        protected void ApplyGravity()
        {
            vel.Y += gravity * elapsed;
        }

        private void Physics()
        {
            //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            pos.X += vel.X;
            //pos.X = (float)Math.Round(pos.X);
            HandlecollisionHorizontal();

            pos.Y += vel.Y;
            //pos.Y = (float)Math.Round(pos.Y);
            HandlecollisionVertical();
            //HandleCollisionsY();
        }

        public Rectangle GetAABB()
        {
            return new Rectangle((int)pos.X + aabb.X, (int)pos.Y + aabb.Y, aabb.Width, aabb.Height);
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

                for (int i = top; i <= bottom; ++i)
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
    }
}
