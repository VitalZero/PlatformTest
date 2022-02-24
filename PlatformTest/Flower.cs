using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Flower : PowerUp
    {
        public Flower(Vector2 pos)
            : base(PowerupType.flower, pos)
        {
            dir = 0f;
            speed = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            if (riseStart <= riseTime)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                riseStart += dt;
                startY += 16.0f * dt;
                yOffset = (int)startY;
            }
            else
            {
                yOffset = 0;
                CanCollide = true;

                LateUpdate(gameTime);
            }
        }
    }
}
