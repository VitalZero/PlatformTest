using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class BouncingTile : Entity
    {
        public bool Done { get; set; }
        private bool bouncing;
        public int TextureID { get; set; }
        private const float upSpeed = -100f;
        private float yVel;
        private readonly float yOrigin;
        private readonly Tile backUpTile;
        public bool CanKill { get; set; }

        public BouncingTile(Tile backUpTile)
        {
            this.backUpTile = backUpTile;
            TextureID = backUpTile.id;
            position.X = backUpTile.X * backUpTile.size;
            position.Y = backUpTile.Y * backUpTile.size;
            size = new Vector2(backUpTile.size, backUpTile.size);
            aabb = new Rectangle(0, 0, (int)size.X, (int)size.Y);
            yOrigin = position.Y;
            Active = true;
            Done = false;
            bouncing = true;
            gravity = 950f;
            CanKill = true;
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

                if (yVel > 0f)
                    CanKill = false;

                yVel += gravity * dt;
                position.Y += yVel * dt;

                if (position.Y >= yOrigin)
                {
                    position.Y = yOrigin;
                    Active = false;
                    Done = true;
                }
            }
        }
    }
}
