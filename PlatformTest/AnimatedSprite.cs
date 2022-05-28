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
            position = pos;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            animPlayer = new AnimationPlayer();
            timeCounter = 0;
            this.lifeTime = lifeTime;
            this.looping = looping;
            IsDestroyed = false;
            origin = Vector2.Zero;

            texture = TextureManager.MiscSprites;

            animPlayer.Add("single", new Animation(texture, frameDuration, looping, (int)frameSize.X, (int)frameSize.Y,
                frameCount, (int)frameStart.X, (int)frameStart.Y));
            animPlayer.PlayAnimation("single");

            if (looping && lifeTime <= 0)
                throw new ArgumentException("If animation is looping, life time must be greater than 0!");
        }

        public void Destroy()
        {
            IsDestroyed = true;
        }

        public override void Init()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch, 
                new Vector2((int)position.X, (int)position.Y),
                SpriteEffects.None, origin, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            velocity += acceleration * elapsed;

            position += velocity * elapsed;

            animPlayer.Update(elapsed);

            if(looping)
            {
                if(timeCounter >= lifeTime)
                {
                    Destroy();
                }

                timeCounter += elapsed;
            }
            else
            {
                if(animPlayer.AnimationEnded("single"))
                {
                    Destroy();
                }
            }

            Vector2 posToScreen = Camera2D.Instance.WorldToScreen(position);

            if (posToScreen.X > 336 || posToScreen.Y > 256 || posToScreen.X < -16 || posToScreen.Y < -16)
            {
                Destroy();
            }
        }
    }
}
