using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Frame
    {
        public Rectangle frameRect;
        public float frameTime;
        public SpriteEffects flip;
        public Rectangle aabb;

        public Frame(Rectangle rect, float frameTime, SpriteEffects flip = SpriteEffects.None, Rectangle? aabb = null)
        {
            frameRect = rect;
            this.frameTime = frameTime;
            this.flip = flip;
            this.aabb = aabb ?? Rectangle.Empty;
        }
    }
}
