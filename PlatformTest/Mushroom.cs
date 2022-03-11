using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Mushroom : PowerUp
    {
        public Mushroom(Vector2 pos)
            :base(PowerupType.mushroom, pos)
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
                position.Y += -15.0f * elapsed;

                if(riseStart > riseTime)
                {
                    CanCollide = true;
                    DrawBehind = false;
                }
            }
            else
            {
                velocity.X = speed * dir;

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
