using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    class StaticSprite : Sprite
    {
        Vector2 frameSize;
        Vector2 frameStart;
        SpriteEffects hFlip;

        public StaticSprite(
               Vector2 pos,
               Vector2 frameSize,
               Vector2 frameStart,
               Texture2D texture,
               float lifeTime = -1f)
        {
            position = pos;
            this.texture = texture;
            this.frameSize = frameSize;
            this.frameStart = frameStart;
            velocity = Vector2.Zero;
            this.lifeTime = lifeTime;
            timeCounter = 0f;
            hFlip = SpriteEffects.None;
            acceleration = Vector2.Zero;
        }

        public void Destroy()
        {
            IsDestroyed = true;
        }

        public override void Init()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (velocity.X > 0)
                hFlip = SpriteEffects.None;
            else if(velocity.X < 0)
                hFlip = SpriteEffects.FlipHorizontally;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            velocity += acceleration * elapsed;

            position += velocity * elapsed;

            if (lifeTime >= 0f)
            {
                if (timeCounter >= lifeTime)
                {
                    Destroy();
                }

                timeCounter += elapsed;
            }

            Vector2 posToScreen = Camera2D.Instance.WorldToScreen(position);

            if (posToScreen.X > 336 || posToScreen.Y > 256 || posToScreen.X < -16 || posToScreen.Y < -16)
            {
                Destroy();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                new Vector2((int)position.X, (int)position.Y),
                new Rectangle((int)frameStart.X, (int)frameStart.Y, (int)frameSize.X, (int)frameSize.Y),
                Color.White, 0f, origin, 1f, hFlip, 0f);
        }
    }
}
