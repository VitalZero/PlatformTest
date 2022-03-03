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
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (riseStart <= riseTime)
            {
                riseStart += elapsed;
                pos.Y += -16.0f * elapsed;
            }
            else
            {
                CanCollide = true;

                LateUpdate(gameTime);
            }
        }
    }
}
