using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class KoopaTrooper : Enemy
    {
        private enum States { wandering, stomped, rebounding, instantKill }

        private AnimationPlayer animPlayer;
        private float awakeTime;
        private float awakeTimeAcc;
        private float deadTime;
        private float deadTimeAcc;
        private States currState;
        private SpriteEffects flip;
        private SpriteEffects vFlip;
        private float angle;
        private float addedAngle;

        public KoopaTrooper(Vector2 pos, int index)
        {
            Index = index;
            this.position = pos;
            aabb = new Rectangle(-6,-12, 12, 12);
            animPlayer = new AnimationPlayer();
            speed = 20f;
            awakeTime = 3f;
            awakeTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = -1f;
            Active = false;
            origin = new Vector2(8, 23);
            angle = 0f;
            addedAngle = MathHelper.TwoPi / 90;
        }

        public override void Init()
        {
            texture = TextureManager.Turtle;

            animPlayer.Add("walking", new Animation(texture, 0.2f, true, 16, 24, 2, 0, 0));
            animPlayer.Add("stomped", new Animation(texture, 1.5f, false, 16, 24, 1, 32, 0));
            animPlayer.Add("awaking", new Animation(texture, 0.25f, true, 16, 24, 2, 32, 0));
            flip = SpriteEffects.None;
            CanKill = true;
            animPlayer.PlayAnimation("walking");
        }


        public override void Hit()
        {
            if (currState == States.wandering)
            {
                currState = States.stomped;
                animPlayer.PlayAnimation("stomped");
                CanKill = false;
            }
            else if (currState == States.stomped)
            {
                currState = States.rebounding;
                animPlayer.PlayAnimation("stomped");
                CanKill = true;
            }
            else if (currState == States.rebounding)
            {
                currState = States.stomped;
                animPlayer.PlayAnimation("stomped");
                CanKill = false;
            }
            //CanCollide = false;
        }

        public override void Kill()
        {
            currState = States.instantKill;
            //origin.Y = 15;
            animPlayer.PlayAnimation("stomped");
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

                        if(velocity.X > 0)
                            flip = SpriteEffects.None;
                        else if(velocity.X < 0)
                            flip = SpriteEffects.FlipHorizontally;

                        if (RightWallHit || LeftWallHit)
                            dir = -dir;
                    }
                    break;
                case States.stomped:
                    {
                        velocity.X = 0;

                        awakeTimeAcc += elapsed;

                        if(awakeTimeAcc >= awakeTime)
                        {
                            awakeTimeAcc = 0f;
                            currState = States.wandering;
                            animPlayer.PlayAnimation("walking");
                            CanKill = true;
                            break;
                        }

                        if(awakeTimeAcc < (awakeTime * 0.5))
                            animPlayer.PlayAnimation("stomped");
                        else
                            animPlayer.PlayAnimation("awaking");
                    }
                    break;
                case States.rebounding:
                    {
                        if(RightWallHit || LeftWallHit)
                        {
                            dir = -dir;
                            SoundManager.Thump.Play();
                        }
                        velocity.X = speed * 15f * dir;
                    }
                    break;
                case States.instantKill:
                    {
                        animPlayer.Freeze();
                        deadTimeAcc += elapsed;

                        angle += addedAngle * dir;

                        angle = Math.Clamp(angle, -MathHelper.Pi, MathHelper.Pi);

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
                new Vector2((int)position.X, (int)position.Y),
                flip | vFlip, origin, Color.White, angle);

            base.Draw(spriteBatch);
        }
    }
}
