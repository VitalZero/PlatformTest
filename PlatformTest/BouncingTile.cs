using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class BouncingTile
    {
        public bool Active { get; set; }
        public bool Done { get; set; }
        private bool bouncing;
        public float X { get; private set; }
        public float Y { get; private set; }
        public int TextureID { get; set; }
        private const float gravity = 30f;
        private const float upSpeed = -200f;
        private float yVel;
        private float yOrigin;
        private Tile backUpTile;

        public BouncingTile(Tile backUpTile)
        {
            this.backUpTile = backUpTile;
            TextureID = backUpTile.id;
            X = backUpTile.X * backUpTile.size;
            Y = backUpTile.Y * backUpTile.size;
            yOrigin = Y;
            Active = true;
            Done = false;
            bouncing = true;
        }

        public Tile Restore()
        {
            return backUpTile;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Active)
            {
                if (bouncing)
                {
                    yVel = upSpeed * dt;
                    bouncing = false;
                }

                yVel += gravity * dt;
                Y += yVel;

                if (Y >= yOrigin)
                {
                    Active = false;
                    Done = true;
                }
            }
        }
    }
}
