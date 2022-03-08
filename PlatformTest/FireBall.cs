using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class FireBall : Entity
    {
        private readonly float bulletSpeed = 180f;
        private readonly float bounceSpeed = -150f;
        private readonly Rectangle destRectangle;

        public FireBall(Vector2 parentPos, float dir)
        {
            pos = parentPos;
            this.dir = dir;
            destRectangle = new Rectangle(20, 4, 8, 8);
            texture = ResourceManager.MiscSprites;
            aabb = new Rectangle(0, 0, 8, 8);
        }

        public override void Update(GameTime gameTime)
        {
            vel.X = bulletSpeed * dir;

            if(isOnGround)
            {
                vel.Y = bounceSpeed;
                isOnGround = false;
            }

            if(LeftWallHit || RightWallHit)
            {
                Destroyed = true;
                CanCollide = false;
                vel = Vector2.Zero;
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
 