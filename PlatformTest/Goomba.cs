using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Goomba : Entity
    {
        private Animation walking;
        private AnimationPlayer animPlayer;

        public Goomba()
        {
            pos = new Vector2(200, 50);
            aabb = new Rectangle(2, 3, 14, 13);
            animPlayer = new AnimationPlayer();
            speed = 20f;
        }

        public void Load(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            texture = content.Load<Texture2D>("goomba");

            walking = new Animation(texture, 0.2f, true, 16, 2, 0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            vel.X = speed * elapsed;

            ApplyGravity();

            animPlayer.PlayAnimation(walking);

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);

            if(RightWallHit || LeftWallHit)
            {
                speed = -speed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                SpriteEffects.None);
        }
    }
}
