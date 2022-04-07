using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Mushroom : PowerUp
    {
        public Mushroom(Vector2 pos)
            :base(ItemType.mushroom, pos)
        {
            dir = 1f;
            speed = 50f;
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

                if (RightWallHit)
                {
                    dir = -1f;
                }
                else if (LeftWallHit)
                {
                    dir = 1f;
                }
                
                LateUpdate(gameTime);

                Vector2 posToScreen = Camera2D.Instance.WorldToScreen(position);

                if (posToScreen.X > 336 || posToScreen.Y > 256 || posToScreen.X < -16 || posToScreen.Y < -16)
                {
                    Destroy();
                }
            }
        }
    }
}
