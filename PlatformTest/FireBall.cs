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
            pos = parentPos;
            this.dir = dir;
            destRectangle = new Rectangle(20, 4, 8, 8);
            texture = ResourceManager.MiscSprites;
            aabb = new Rectangle(2, 2, 4, 4);
            vel.Y = bulletSpeed / 1.1f;
            AffectedByGravity = false;
            gravity = 1500;
        }

        public override void Update(GameTime gameTime)
        {
            vel.X = bulletSpeed * dir;

            if(IsOnGround)
            {
                AffectedByGravity = true;
                vel.Y = bounceSpeed;
                IsOnGround = false;
            }

            if(LeftWallHit || RightWallHit)
            {
                Destroyed = true;
                CanCollide = false;
                vel = Vector2.Zero;

                if(RightWallHit)
                    SpriteManager.Add(new AnimatedSprite(
                    new Vector2(pos.X /*- destRectangle.Width / 2*/, pos.Y - destRectangle.Height / 2),
                    new Vector2(16, 16), new Vector2(16, 0), 0.03f, 3));
                else if (LeftWallHit)
                    SpriteManager.Add(new AnimatedSprite(
                    new Vector2(pos.X - destRectangle.Width /*- destRectangle.Width / 2*/, pos.Y - destRectangle.Height / 2),
                    new Vector2(16, 16), new Vector2(16, 0), 0.03f, 3));
            }

            LateUpdate(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - Camera.Instance.YOffset),
                destRectangle,
                Color.White);
        }
    }
}
 