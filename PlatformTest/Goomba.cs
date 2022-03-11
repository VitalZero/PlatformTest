using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Goomba : Enemy
    {
        private enum States { wandering, stomped, instantKill }

        private AnimationPlayer animPlayer;
        private float deadTime;
        private float deadTimeAcc;
        private States currState;
        private SpriteEffects vFlip;

        public Goomba(Vector2 pos, int index)
        {
            Index = index;
            this.position = pos;
            aabb = new Rectangle(-6, -12, 12, 12);
            animPlayer = new AnimationPlayer();
            speed = 20f;
            deadTime = 0.5f;
            deadTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = -1f;
            Active = false;
            vFlip = SpriteEffects.None;
            origin = new Vector2(8, 15);
        }

        public override void Init()
        {
            texture = ResourceManager.Goomba;

            animPlayer.Add("walking", new Animation(texture, 0.2f, true, 16, 16, 2, 0, 0));
            animPlayer.Add("stomped", new Animation(texture, 1f, false, 16, 16, 1, 32, 0));
            animPlayer.PlayAnimation("walking");
        }

        public override void Hit()
        {
            currState = States.stomped;
            animPlayer.PlayAnimation("stomped");
            CanKill = false;
            CanCollide = false;
        }

        public override void Kill()
        {
            currState = States.instantKill;
            velocity.Y = -250f;
            CanKill = false;
            CanCollide = false;
            speed = 30f;
            vFlip = SpriteEffects.FlipVertically;
            IsOnGround = false;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (currState)
            {
                case States.wandering:
                    {
                        velocity.X = speed * dir;

                        if (RightWallHit)
                        {
                            dir = -1f;
                        }
                        else if(LeftWallHit)
                        {
                            dir = 1f;
                        }
                    }
                    break;
                case States.stomped:
                    {
                        deadTimeAcc += elapsed;
                        velocity.X = 0f;

                        if (deadTimeAcc >= deadTime)
                        {
                            Active = false;
                            IsDestroyed = true;
                        }
                    }
                    break;
                case States.instantKill:
                    {
                        animPlayer.Freeze();
                        deadTimeAcc += elapsed;

                        velocity.X = speed * dir;

                        if (deadTimeAcc >= 1f)
                        {
                            Active = false;
                            IsDestroyed = true;
                        }
                    }
                    break;
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
            animPlayer.Draw(spriteBatch,
                new Vector2((int)position.X - (int)Camera.Instance.XOffset, (int)position.Y - (int)Camera.Instance.YOffset),
                vFlip, origin);

            base.Draw(spriteBatch);
        }
    }
}
