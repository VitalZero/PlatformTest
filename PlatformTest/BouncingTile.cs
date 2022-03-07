using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class BouncingTile : Entity
    {
        //public bool Active { get; set; }
        public bool Done { get; set; }
        private bool bouncing;
        public int TextureID { get; set; }
        //private const float gravity = 850f;
        private const float upSpeed = -100f;
        private float yVel;
        private readonly float yOrigin;
        private readonly Tile backUpTile;

        public BouncingTile(Tile backUpTile)
        {
            this.backUpTile = backUpTile;
            TextureID = backUpTile.id;
            pos.X = backUpTile.X * backUpTile.size;
            pos.Y = backUpTile.Y * backUpTile.size;
            size = new Vector2(backUpTile.size, backUpTile.size);
            aabb = new Rectangle(0, 0, (int)size.X, (int)size.Y);
            yOrigin = pos.Y;
            Active = true;
            Done = false;
            bouncing = true;
            gravity = 850f;
        }

        public Tile Restore()
        {
            return backUpTile;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Active)
            {
                if (bouncing)
                {
                    yVel = upSpeed;
                    bouncing = false;
                }

                yVel += gravity * dt;
                pos.Y += yVel * dt;

                if (pos.Y >= yOrigin)
                {
                    pos.Y = yOrigin;
                    Active = false;
                    Done = true;
                }
            }
        }
    }
}
