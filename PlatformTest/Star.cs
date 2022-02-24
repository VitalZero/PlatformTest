using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Star : PowerUp
    {
        public float bounceSpeed = -450f;
        public Star(Vector2 pos)
            : base(PowerupType.star, pos)
        {
            dir = 1f;
            speed = 80f;
            gravity = 20f;
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
