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
            speed = 40f;
            DrawBehind = true;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (riseStart <= riseTime)
            {
                riseStart += elapsed;
                pos.Y += -15.0f * elapsed;
                
                if (riseStart > riseTime)
                {
                    CanCollide = true;
                    DrawBehind = false;
                }
            }
            else
            {
                vel.X = speed * dir;

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
