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
        private float angle;
        private float addedAngle;
        private bool killed = false;

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
            angle = 0f;
            addedAngle = MathHelper.TwoPi / 90;
        }

        public override void Init()
        {
            texture = TextureManager.Goomba;

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
            killed = true;
            currState = States.instantKill;
            velocity.Y = -250f;
            CanKill = false;
            CanCollide = false;
            speed = 30f;
            //vFlip = SpriteEffects.FlipVertically;
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
                        angle += addedAngle * dir;

                        angle = Math.Clamp(angle, -MathHelper.Pi, MathHelper.Pi);

                        animPlayer.Freeze();
                        deadTimeAcc += elapsed;

                        velocity.X = speed * 1.5f * dir;

                        if (deadTimeAcc >= 1f)
                        {
                            Destroy();
                        }
                    }
                    break;
            }

            animPlayer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            LateUpdate(gameTime);

            Vector2 posToScreen = Camera2D.Instance.WorldToScreen(position);

            if (posToScreen.X > 336 || posToScreen.Y > 256 || posToScreen.X < -16 || posToScreen.Y < -16)
            {
                Destroy();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch,
                new Vector2((int)position.X, (int)position.Y ),
                vFlip, origin, angle);

            base.Draw(spriteBatch);
        }
    }
}
