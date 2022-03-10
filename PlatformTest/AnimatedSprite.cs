using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class AnimatedSprite : Sprite
    {
        private AnimationPlayer animPlayer;
        private bool looping;

        public AnimatedSprite(
            Vector2 pos,
            Vector2 frameSize,
            Vector2 frameStart,
            float frameDuration,
            int frameCount,
            bool looping = false,
            float lifeTime = 0f)
        {
            this.pos = pos;
            vel = Vector2.Zero;
            destroyed = false;
            animPlayer = new AnimationPlayer();
            timeCounter = 0;
            this.lifeTime = lifeTime;
            this.looping = looping;
            destroyed = false;
            origin = Vector2.Zero;

            texture = ResourceManager.MiscSprites;

            animPlayer.Add("single", new Animation(texture, frameDuration, looping, (int)frameSize.X, (int)frameSize.Y,
                frameCount, (int)frameStart.X, (int)frameStart.Y));
            animPlayer.PlayAnimation("single");

            if (looping && lifeTime <= 0)
                throw new ArgumentException("If animation is looping, life time must be greater than 0!");
        }

        public void SetVelocity(Vector2 velocity)
        {
            vel = velocity;
        }

        public void Destroy()
        {
            destroyed = true;
        }

        public override void Init()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch, new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                SpriteEffects.None, origin);
        }

        public override void Update(GameTime gameTime)
        {
            pos += vel;

            animPlayer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if(looping)
            {
                if(timeCounter >= lifeTime)
                {
                    destroyed = true;
                }
            }
            else
            {
                if(animPlayer.AnimationEnded("single"))
                {
                    destroyed = true;
                }
            }
        }
    }
}
