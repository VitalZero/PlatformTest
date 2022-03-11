#define DEBUG_DRAW

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PlatformTest
{
    public class Entity : GameObject
    {
        protected Vector2 velocity;
        protected float speed;
        protected float gravity = 850f;
        protected bool IsOnGround;
        protected bool RightWallHit;
        protected bool LeftWallHit;
        protected bool FloorHit;
        protected bool CeilingHit;
        protected float scale;
        protected float dir;
        public bool Active { get; set; }
        public bool CanCollide { get; set; }
        public bool CanBeHit { get; set; }
        public bool DrawBehind { get; set; }
        public bool AffectedByGravity { get; set; }
        public short drawPriority; 

        private Point tileHit;

        public Vector2 Position { get { return position; } }

        public Entity()
        {
            IsOnGround = false;
            tileHit = Point.Zero;
            Active = true;
            CanCollide = true;
            IsDestroyed = false;
            DrawBehind = false;
            AffectedByGravity = true;
            drawPriority = 0;
            CanBeHit = true;
            scale = 1;
        }

        public Rectangle GetAABB()
        {
            return new Rectangle((int)Position.X + aabb.X, (int)Position.Y + aabb.Y, aabb.Width, aabb.Height);
        }

        public void Move(float x, float y)
        {
            position.X += x;
            position.Y += y;
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
                (int)(aabbDebug.Center.Y),
                1, 1),
                new Color(Color.Yellow, 0.8f));

            spriteBatch.Draw(ResourceManager.Pixel, new Rectangle(
                            (int)position.X - (int)Camera.Instance.XOffset, (int)position.Y - (int)Camera.Instance.YOffset,
                            1, 1),
                            Color.Red);
#endif
        }

        public virtual void Init()
        {}

        public virtual void Hit()
        {}

        public void Destroy()
        {
            Active = false;
            IsDestroyed = true;
        }

        public virtual void Kill()
        {}

        public override void Update(GameTime gameTime)
        {
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

            if (!IsOnGround && AffectedByGravity)
            {
                ApplyGravity(elapsed);
                velocity.Y = MathHelper.Clamp(velocity.Y, -400, 400);
            }

            Physics(elapsed);
        }

        protected Point GetContactTile()
        {
            return tileHit;
        }

        protected void ApplyGravity(float elapsed)
        {
            velocity.Y += gravity * elapsed;
        }

        private void Physics(float elapsed)
        {
            position.X += velocity.X * elapsed;

            if(CanCollide)
                HandlecollisionHorizontal();

            position.Y += velocity.Y * elapsed;

            if (CanCollide)
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
            if (velocity.X > 0)
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

                            position.X -= depth;
                            position.X = (int)position.X;
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
            else if (velocity.X < 0)
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

                            position.X -= depth;
                            position.X = (int)position.X;
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
            if((int)velocity.Y == 0 && IsOnGround)
            {
                // if solid tiles are found on the bottom (left and right), means we are on ground
                // dont apply gravity and get out of the function, we dont need to check anything the floor again or the ceiling
                if(World.Instance.GetTile(left, bottom).collision > TileCollision.none ||
                    World.Instance.GetTile(right, bottom).collision > TileCollision.none)
                {
                    IsOnGround = true;
                    return;
                }
                // if the entity doesnt find a solid tile on the feet (left and right side of the bounding box)
                // means that is not on ground and can start to apply gravity
                else 
                {
                    IsOnGround = false;
                }
            }

            // if we're going down, check the bottom tiles from left to right
            if (velocity.Y > 0)
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

                            position.Y -= depth;
                            position.Y = (int)position.Y;
                            IsOnGround = true;
                            FloorHit = true;
                            velocity.Y = 0;
                        }
                    }
                    else
                    {
                        bounds = GetAABB();

                        if (bounds.Bottom > (t.Y * 16) + 1)
                        {
                            IsOnGround = false;
                        }
                    }
                }
            }
            // if we're going up, check the top tiles from left to right
            else if (velocity.Y < 0)
            {
                List<Tile> tilesToCheck = new List<Tile>();

                for (int i = left; i <= right; ++i)
                {
                    tilesToCheck.Add(World.Instance.GetTile(i, top));
                }

                //if (dir < 0f)
                //    tilesToCheck.Reverse();

                foreach (var t in tilesToCheck)
                {
                    if (t.collision != TileCollision.none)
                    {
                        bounds = GetAABB();
                        Rectangle tileBounds = World.Instance.GetBounds(t.X, t.Y);

                        if (bounds.Intersects(tileBounds))
                        {
                            int depth = bounds.Top - tileBounds.Bottom;
                            tileHit = new Point(t.X, t.Y);

                            position.Y -= depth;
                            position.Y = (int)position.Y;
                            CeilingHit = true;
                            velocity.Y = 0;
                            break;
                        }
                    }
                }
            }
        }
    }
}
