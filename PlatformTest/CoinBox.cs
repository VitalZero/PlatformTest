using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class CoinBox : Item
    {
        private bool rising;
        private float yOrigin;
        private AnimationPlayer animPlayer;

        public CoinBox(Vector2 pos)
            : base(ItemType.coinBox, pos)
        {
            speed = 250;
            dir = 0f;
            CanCollide = false;
            this.position.Y -= 16f;
            this.position.X += 4f;
            rising = true;

            yOrigin = pos.Y;
        }

        public override void Init()
        {
            texture = TextureManager.MiscSprites;
            animPlayer = new AnimationPlayer();
            animPlayer.Add("idle", new Animation(0.05f, true, 8, 16, 4, 64, 0));
            spriteArea = new Rectangle(0, 0, 16, 16);

            animPlayer.PlayAnimation("idle");
        }

        public override void Update(GameTime gameTime)
        {
            startY = 16f;
            yOffset = 0;

            if(rising)
            {
                velocity.Y = -speed;
                rising = false;
            }

            velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(position.Y >= yOrigin - 16f)
            {
                Destroy();
            }


            animPlayer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            //LateUpdate(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(
                spriteBatch,
                texture,
                new Vector2((int)position.X, (int)position.Y),
                SpriteEffects.None, 
                Vector2.Zero, 
                Color.White);
        }
    }
}
