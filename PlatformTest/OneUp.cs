using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class OneUp : PowerUp
    {
        public OneUp(Vector2 pos)
                    : base(PowerupType.oneup, pos)
        {
            dir = 1f;
            speed = 30f;
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
                vel.X = speed * dir;
                CanCollide = true;

                if (RightWallHit)
                {
                    dir = -1f;
                }
                else if (LeftWallHit)
                {
                    dir = 1f;
                }

                LateUpdate(gameTime);
            }
        }
    }
}
