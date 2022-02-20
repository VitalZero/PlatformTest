using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    class AnimationPlayer
    {
        int frameIndex;
        public int FrameIndex { get { return frameIndex; } }
        //public Vector2 Origin { get { return new Vector2(Animation.FrameWidth / 2, Animation.FrameHeight); } }
        float time;
        Rectangle source;
        private bool freeze;
        public EventHandler AnimationEnded;
        private bool animationEnded;
        private Dictionary<string, Animation> animations;
        string currentAnimation;

        public AnimationPlayer()
        {
            animations = new Dictionary<string, Animation>();
        }

        public void PlayAnimation(string animationName)
        {
            if(!animations.ContainsKey(animationName))
                throw new NotSupportedException("The animation doesnt exist!");

            if (currentAnimation != animationName)
            {
                currentAnimation = animationName;
                frameIndex = 0;
                time = 0f;
                animationEnded = false;

                source = new Rectangle(
                    (FrameIndex * animations[currentAnimation].FrameWidth) + animations[currentAnimation].StartFrameX, 
                    0, 
                    animations[currentAnimation].FrameWidth,
                    animations[currentAnimation].FrameHeight);
            }
        }

        public void Add(string animationName, Animation animation)
        {
            if(!animations.ContainsKey(animationName))
            {
                animations.Add(animationName, animation);
            }
        }

        public void Freeze()
        {
            freeze = true;
        }

        public void OnAnimationEnded()
        {
            if(AnimationEnded != null)
            {
                AnimationEnded(this, EventArgs.Empty);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (animations.Count == 0)
                throw new NotSupportedException("The player doesnt contain any animation!");

            if (!freeze)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;

                while (time > animations[currentAnimation].FrameTime)
                {
                    time -= animations[currentAnimation].FrameTime;

                    if (animations[currentAnimation].IsLooping)
                    {
                        frameIndex = (frameIndex + 1) % animations[currentAnimation].FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(frameIndex + 1, animations[currentAnimation].FrameCount - 1);
                        if (frameIndex + 1 >= animations[currentAnimation].FrameCount)
                            OnAnimationEnded();
                    }
                }
            }
            else
            {
                freeze = false;
            }

            source = new Rectangle(
                (FrameIndex * animations[currentAnimation].FrameWidth) + animations[currentAnimation].StartFrameX, 
                0,
                animations[currentAnimation].FrameWidth,
                animations[currentAnimation].FrameHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos, SpriteEffects effects)
        {
            spriteBatch.Draw(animations[currentAnimation].Texture, pos, source, Color.White, 
                0f, Vector2.Zero, 1f, effects, 0f);
        }
    }
}
