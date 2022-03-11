using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class FireBall : Entity
    {
        private readonly float bulletSpeed = 250f;
        private readonly float bounceSpeed = -200f;
        private readonly Rectangle destRectangle;
        private AnimationPlayer animPlayer;
        SpriteEffects hFlip;

        public FireBall(Vector2 parentPos, float dir)
        {
            position = parentPos;
            this.dir = dir;
            destRectangle = new Rectangle(20, 4, 8, 8);
            aabb = new Rectangle(-2, -2, 4, 4);
            velocity.Y = bulletSpeed / 1.1f;
            AffectedByGravity = false;
            gravity = 1500;
            animPlayer = new AnimationPlayer();
            origin = new Vector2(4, 4);

            if (dir > 0)
                hFlip = SpriteEffects.None;
            else if (dir < 0)
                hFlip = SpriteEffects.FlipHorizontally;
        }

        public override void Init()
        {
            texture = ResourceManager.MiscSprites;
            animPlayer.Add("normal", new Animation(texture, 0.06f, true, 8, 8, 4, 8, 16));
            animPlayer.PlayAnimation("normal");
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
                    new Vector2(position.X - 4, position.Y - 8),
                    new Vector2(16, 16), new Vector2(16, 0), 0.03f, 3));
                else if (LeftWallHit)
                    SpriteManager.Add(new AnimatedSprite(
                    new Vector2(position.X - 12, position.Y - 8),
                    new Vector2(16, 16), new Vector2(16, 0), 0.03f, 3));
            }

            animPlayer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            LateUpdate(gameTime);

            if (position.Y > ((World.Instance.mapHeight + 3) * 16))
            {
                IsDestroyed = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch, new Vector2((int)position.X - (int)Camera.Instance.XOffset, (int)position.Y - Camera.Instance.YOffset),
                hFlip, origin);
        }
    }
}
 