using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class FireBall : Entity
    {
        private readonly float bulletSpeed = 200f;
        private readonly float bounceSpeed = -200f;
        private readonly Rectangle destRectangle;

        public FireBall(Vector2 parentPos, float dir)
        {
            position = parentPos;
            this.dir = dir;
            destRectangle = new Rectangle(20, 4, 8, 8);
            texture = ResourceManager.MiscSprites;
            aabb = new Rectangle(2, 2, 4, 4);
            velocity.Y = bulletSpeed / 1.1f;
            AffectedByGravity = false;
            gravity = 1500;
        }

        public override void Update(GameTime gameTime)
        {
            velocity.X = bulletSpeed * dir;

            if(IsOnGround)
            {
                AffectedByGravity = true;
                velocity.Y = bounceSpeed;
                IsOnGround = false;
            }

            if(LeftWallHit || RightWallHit)
            {
                IsDestroyed = true;
                CanCollide = false;
                velocity = Vector2.Zero;

                if(RightWallHit)
                    SpriteManager.Add(new AnimatedSprite(
                    new Vector2(position.X /*- destRectangle.Width / 2*/, position.Y - destRectangle.Height / 2),
                    new Vector2(16, 16), new Vector2(16, 0), 0.03f, 3));
                else if (LeftWallHit)
                    SpriteManager.Add(new AnimatedSprite(
                    new Vector2(position.X - destRectangle.Width /*- destRectangle.Width / 2*/, position.Y - destRectangle.Height / 2),
                    new Vector2(16, 16), new Vector2(16, 0), 0.03f, 3));
            }

            LateUpdate(gameTime);
            if (position.Y > ((World.Instance.mapHeight + 3) * 16))
            {
                IsDestroyed = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                new Vector2((int)position.X - (int)Camera.Instance.XOffset, (int)position.Y - Camera.Instance.YOffset),
                destRectangle,
                Color.White);
        }
    }
}
 