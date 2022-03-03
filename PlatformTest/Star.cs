using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Star : PowerUp
    {
        public float bounceSpeed = -400f;
        public Star(Vector2 pos)
            : base(PowerupType.star, pos)
        {
            dir = 1f;
            speed = 80f;
            gravity = 16f;
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
                vel.X = speed * elapsed * dir;

                if(isOnGround)
                {
                    vel.Y = bounceSpeed * elapsed;
                    isOnGround = false;
                }

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
