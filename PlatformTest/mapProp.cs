using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class BouncingTile
    {
        public bool Active { get; set; }
        public bool Done { get; set; }
        private bool bouncing;
        public float X { get; private set; }
        public float Y { get; private set; }
        public int TextureID { get; set; }
        private const float gravity = 30f;
        private const float upSpeed = -200f;
        private float yVel;
        private float yOrigin;
        //float time;
        //float duration;

        public BouncingTile(int textureID, float x, float y)
        {
            TextureID = textureID;
            X = x;
            Y = y;
            yOrigin = y;
            Active = true;
            Done = false;
            bouncing = true;
            //time = 0;
            //duration = .3f;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Active)
            {
                //Y = Lerp(yOrigin, yOrigin - 16f, EaseInOut(time/duration));

                //if(time <= duration)
                //    time += dt;
                if (bouncing)
                {
                    yVel = upSpeed * dt;
                    bouncing = false;
                }

                yVel += gravity * dt;
                Y += yVel;

                if (Y >= yOrigin)
                {
                    Active = false;
                    Done = true;
                }
            }
        }

        private float Lerp(float startValue, float endValue, float t)
        {
            if (t > 0.5f)
            {
                swap(ref startValue, ref endValue);
            }

            if (t <= 0)
                return startValue;

            if (t >= 1f)
                return endValue;

            return (startValue + (endValue - startValue) * t);
        }

        private float EaseIn(float t)
        {
            return t * t;
        }

        private float EaseInOut(float t)
        {
            if (t <= 0.5f)
                return 2 * t * t;
            else
                return 1 - (((-2 * t + 2) * (-2 * t + 2)) / 2);
        }

        private void swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

    }
}
