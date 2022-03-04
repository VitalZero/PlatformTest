#define DEBUG_DRAW

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PlatformTest
{
    public class Entity : GameObject
    {
        protected Vector2 vel;
        protected float speed;
        protected float gravity = 850f;
        protected bool isOnGround;
        protected bool RightWallHit;
        protected bool LeftWallHit;
        protected bool FloorHit;
        protected bool CeilingHit;
        public bool Active { get; set; }
        public bool CanCollide { get; set; }
        public bool Destroyed { get; set; }
        protected float dir;

        private Point tileHit;

        public Vector2 Pos { get { return pos; } }

        public Entity()
        {
            isOnGround = false;
            tileHit = Point.Zero;
            Active = true;
            CanCollide = true;
            Destroyed = false;
        }

        public void Move(float x, float y)
        {
            pos.X += x;
            pos.Y += y;
        }

        public void SetDir(int dir)
        {
            this.dir = dir;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

#if DEBUG_DRAW
            Rectangle aabbDebug = GetAABB();
            aabbDebug.X += (int)-Camera.Instance.XOffset;
            aabbDebug.Y += (int)-Camera.Instance.YOffset;
            spriteBatch.Draw(ResourceManager.Pixel, aabbDebug, new Color(Color.Blue, 0.5f));

            spriteBatch.Draw(ResourceManager.Pixel, new Rectangle(
                (int)(aabbDebug.Center.X),
                (int)(aabbDebug.Bottom),
                1, 1),
                new Color(Color.White, 0.8f)) ;

            spriteBatch.Draw(ResourceManager.Pixel, new Rectangle(
                (int)(aabbDebug.Center.X),
                (int)(aabbDebug.Center.Y),
                1, 1),
                new Color(Color.Yellow, 0.8f));
#endif
        }

        public virtual void Init()
        {}

        public virtual void Hit()
        {}

        public override void Update(GameTime gameTime)
        {
        }

        public void Kill()
        {
            Active = false;
            Destroyed = true;
        }

        protected void LateUpdate(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // deactivate flags
            RightWallHit = false;
            LeftWallHit = false;
            FloorHit = false;
            CeilingHit = false;
            tileHit = Point.Zero;

            if (!isOnGround)
                ApplyGravity(elapsed);

            //vel.Y = MathHelper.Clamp(vel.Y, (-256 * elapsed), (256 * elapsed));

            Physics(elapsed);
        }

        protected Point GetContactTile()
        {
            return tileHit;
        }

        protected void ApplyGravity(float elapsed)
        {
            vel.Y += gravity * elapsed;
        }

        private void Physics(float elapsed)
        {
            pos.X += vel.X * elapsed;
            HandlecollisionHorizontal();

            pos.Y += vel.Y * elapsed;
            HandlecollisionVertical();
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
                    tilesToCheck.Add(World.Instance.GetTile(right, i));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = World.Instance.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            int depth = bounds.Right - tileBounds.Left;

                            pos.X -= depth;
                            pos.X = (int)pos.X;
                            RightWallHit = true;
                            break;
                        }
                    }
                    else
                    {
                        RightWallHit = false;
                    }
                }
            }
            // if we're going left, check all the tiles to the left, from top to bottom
            else if (vel.X < 0)
            {
                List<Tile> tilesToCheck = new List<Tile>();

                for (int i = top; i <= bottom; ++i)
                {
                    tilesToCheck.Add(World.Instance.GetTile(left, i));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = World.Instance.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            int depth = bounds.Left - tileBounds.Right;

                            pos.X -= depth;
                            pos.X = (int)pos.X;
                            LeftWallHit = true;
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

            //check if we are on the ground and the velocity is 0 (we are not falling or jumping)
            if((int)vel.Y == 0 && isOnGround)
            {
                // if solid tiles are found on the bottom (left and right), means we are on ground
                // dont apply gravity and get out of the function, we dont need to check anything the floor again or the ceiling
                if(World.Instance.GetTile(left, bottom).collision > TileCollision.none ||
                    World.Instance.GetTile(right, bottom).collision > TileCollision.none)
                {
                    isOnGround = true;
                    return;
                }
                // if the entity doesnt find a solid tile on the feet (left and right side of the bounding box)
                // means that is not on ground and can start to apply gravity
                else 
                {
                    isOnGround = false;
                }
            }

            // if we're going down, check the bottom tiles from left to right
            if (vel.Y > 0)
            {
                List<Tile> tilesToCheck = new List<Tile>();

                for (int i = left; i <= right; ++i)
                {
                    tilesToCheck.Add(World.Instance.GetTile(i, bottom));
                }

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = World.Instance.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            int depth = bounds.Bottom - tileBounds.Top;

                            pos.Y -= depth;
                            pos.Y = (int)pos.Y;
                            isOnGround = true;
                            FloorHit = true;
                            vel.Y = 0;
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
                    tilesToCheck.Add(World.Instance.GetTile(i, top));
                }

                if (dir < 0f)
                    tilesToCheck.Reverse();

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = World.Instance.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            int depth = bounds.Top - tileBounds.Bottom;

                            //if (t.collision == TileCollision.breakable)
                            //    map.RemoveTile(t.X, t.Y);
                            //else if (t.collision == TileCollision.item)
                            //    map.usedTileItem(t.X, t.Y);
                            tileHit = new Point(t.X, t.Y);

                            pos.Y -= depth;
                            pos.Y = (int)pos.Y;
                            CeilingHit = true;
                            vel.Y = 0;
                            break;
                        }
                    }
                }
            }
        }
    }
}
