using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Flower : Item
    {
        public Flower(Vector2 pos)
            : base(ItemType.flower, pos)
        {
            dir = 0f;
            speed = 0f;
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
                LateUpdate(gameTime);
            }
        }
    }
}
