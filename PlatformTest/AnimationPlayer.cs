using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    class AnimationPlayer
    {
        Animation animation;
        public Animation Animation { get { return animation; } }
        int frameIndex;
        public int FrameIndex { get { return frameIndex; } }
        //public Vector2 Origin { get { return new Vector2(Animation.FrameWidth / 2, Animation.FrameHeight); } }
        float time;
        Rectangle source;
        private bool freeze;

        public void PlayAnimation(Animation animation)
        {
            if(Animation != animation)
            {
                this.animation = animation;
                this.frameIndex = 0;
                this.time = 0f;

                freeze = false;

                source = new Rectangle((FrameIndex * Animation.FrameWidth) + animation.StartFrameX, 0, Animation.FrameWidth, Animation.FrameHeight);
            }
        }

        public void Freeze()
        {
            freeze = true;
        }

        public void Update(GameTime gameTime)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing!");

            if (!freeze)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;

                while (time > Animation.FrameTime)
                {
                    time -= Animation.FrameTime;

                    if (Animation.IsLooping)
                    {
                        frameIndex = (frameIndex + 1) % Animation.FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                    }
                }
            }
            else
            {
                freeze = false;
            }

                source = new Rectangle((FrameIndex * Animation.FrameWidth) + animation.StartFrameX, 0, Animation.FrameWidth, Animation.FrameHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos, SpriteEffects effects)
        {
            spriteBatch.Draw(Animation.Texture, pos, source, Color.White, 0f, Vector2.Zero, 1f, effects, 0f);
        }
    }
}
