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
        private bool ended;
        //public EventHandler AnimationEnded;
        private Dictionary<string, Animation> animations;
        string currentAnimation;

        public AnimationPlayer()
        {
            animations = new Dictionary<string, Animation>();
            frameIndex = 0;
            ended = false;
            time = 0f;
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
                ended = false;

                source.X = (FrameIndex * animations[currentAnimation].FrameWidth) + animations[currentAnimation].StartFrameX;
                source.Y = animations[currentAnimation].StartFrameY;
                source.Width = animations[currentAnimation].FrameWidth;
                source.Height = animations[currentAnimation].FrameHeight;

                //source = new Rectangle(
                //    (FrameIndex * animations[currentAnimation].FrameWidth) + animations[currentAnimation].StartFrameX,
                //    animations[currentAnimation].StartFrameY, 
                //    animations[currentAnimation].FrameWidth,
                //    animations[currentAnimation].FrameHeight);
            }
        }

        public void Step(float delta)
        {
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

        public bool AnimationEnded(string animationName)
        {
            if (ended && animationName == currentAnimation)
                return true;

            return false;
        }

        public string CurrentAnimation()
        {
            return currentAnimation;
        }

        //public void OnAnimationEnded()
        //{
        //    if(AnimationEnded != null)
        //    {
        //        AnimationEnded(this, EventArgs.Empty);
        //    }
        //}

        public void Update(float dt)
        {
            if (animations.Count == 0)
                throw new NotSupportedException("The player doesnt contain any animation!");

            if (!freeze)
            {
                time += dt;

                while (time > animations[currentAnimation].CurrentFrame.frameTime)
                {
                    time -= animations[currentAnimation].CurrentFrame.frameTime;

                    animations[currentAnimation].Advance();
                    ended = animations[currentAnimation].AnimationEnded;

                    //if (animations[currentAnimation].IsLooping)
                    //{
                    //    frameIndex = (frameIndex + 1) % animations[currentAnimation].FrameCount;
                    //}
                    //else
                    //{
                    //    if (frameIndex + 1 >= animations[currentAnimation].FrameCount)
                    //        ended = true;

                    //    frameIndex = Math.Min(frameIndex + 1, animations[currentAnimation].FrameCount - 1);
                    //}
                }
            }
            else
            {
                freeze = false;
            }

            //source = new Rectangle(
            //    (FrameIndex * animations[currentAnimation].FrameWidth) + animations[currentAnimation].StartFrameX,
            //    animations[currentAnimation].StartFrameY,
            //    animations[currentAnimation].FrameWidth,
            //    animations[currentAnimation].FrameHeight);
            source = new Rectangle(
                animations[currentAnimation].CurrentFrame.frameRect.X,
                animations[currentAnimation].CurrentFrame.frameRect.Y,
                animations[currentAnimation].CurrentFrame.frameRect.Width,
                animations[currentAnimation].CurrentFrame.frameRect.Height);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos, SpriteEffects effects, Vector2 origin)
        {
            spriteBatch.Draw(animations[currentAnimation].Texture, pos, source, Color.White, 
                0f, origin, 1f, effects, 1f);
        }
    }
}
