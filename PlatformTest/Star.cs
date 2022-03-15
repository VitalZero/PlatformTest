using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Star : PowerUp
    {
        public float bounceSpeed = -300f;
        public Star(Vector2 pos)
            : base(ItemType.star, pos)
        {
            dir = 1f;
            speed = 70f;
            DrawBehind = true;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (riseStart <= riseTime)
            {
                riseStart += elapsed;
                position.Y += -15.0f * elapsed;

                if (riseStart > riseTime)
                {
                    CanCollide = true;
                    DrawBehind = false;
                }
            }
            else
            {
                velocity.X = speed * dir;

                if(IsOnGround)
                {
                    velocity.Y = bounceSpeed;
                    IsOnGround = false;
                }

                if (RightWallHit)
                {
                    dir = -1f;
                }
                else if (LeftWallHit)
                {
                    dir = 1f;
                }

                LateUpdate(gameTime);
                if (position.Y > ((World.Instance.mapHeight + 3) * 16))
                {
                    IsDestroyed = true;
                }
            }
        }
    }
}
