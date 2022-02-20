#define DEBUG_DRAW

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PlatformTest
{
    public class Entity : GameObject
    {
        protected Vector2 vel;
        protected float speed;
        protected float gravity = 20f;
        protected float jumpSpeed;
        protected bool isOnGround;
        protected float elapsed;
        protected bool RightWallHit;
        protected bool LeftWallHit;
        protected bool FloorHit;
        protected bool CeilingHit;
        public bool Active { get; set; }
        public bool CanCollide { get; set; }
        public bool CanKill { get; set; }
        protected float dir;

        private Point tileHit;

        public Vector2 Pos { get { return pos; }  }

        public Entity()
        {
            isOnGround = false;
            elapsed = 0;
            tileHit = Point.Zero;
            Active = true;
            CanCollide = true;
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
            //spriteBatch.Draw(texture, 
            //    new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
            //    Color.White);

#if DEBUG_DRAW
            Rectangle aabbDebug = GetAABB();
            aabbDebug.X += (int)-Camera.Instance.XOffset;
            aabbDebug.Y += (int)-Camera.Instance.YOffset;
            spriteBatch.Draw(ResourceManager.Pixel, aabbDebug, new Color(Color.Blue, 0.5f));

            //spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)Camera.Instance.XOffset, ((aabbDebug.Top / 16) * 16) - (int)Camera.Instance.YOffset, 16, 16),
            //    new Color(Color.White, 0.2f));
            //spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)Camera.Instance.XOffset, ((aabbDebug.Bottom / 16) * 16) - (int)Camera.Instance.YOffset, 16, 16),
            //    new Color(Color.Green, 0.2f));

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

            //spriteBatch.DrawString(ResourceManager.Arial, playerStates[(int)currState], 
            //    new Vector2((int)pos.X - 10 - (int)Camera.Instance.XOffset, (int)pos.Y - 20 - (int)Camera.Instance.YOffset), Color.Crimson);
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
        }

        protected void LateUpdate(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsed *= 0.5f;
            RightWallHit = false;
            LeftWallHit = false;
            FloorHit = false;
            CeilingHit = false;
            tileHit = Point.Zero;

            for (int i = 0; i < 2; ++i)
            {

                Physics();
            }
        }

        protected Point GetContactTile()
        {
            return tileHit;
        }

        protected void ApplyGravity()
        {
            vel.Y += gravity;
        }

        private void Physics()
        {
            pos.X += vel.X * elapsed;
            //pos.X = (float)Math.Round(pos.X);
            HandlecollisionHorizontal();

            vel.Y = MathHelper.Clamp(vel.Y, -500f, 500f);
            pos.Y += vel.Y * elapsed;
            //pos.Y = (float)Math.Round(pos.Y);
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
