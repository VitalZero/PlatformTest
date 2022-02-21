using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class KoopaTrooper : Entity
    {
        private enum States { wandering, stomped, rebounding }

        private AnimationPlayer animPlayer;
        private float awakeTime;
        private float awakeTimeAcc;
        private States currState;
        private SpriteEffects flip;

        public KoopaTrooper(Vector2 pos)
        {
            this.pos = pos;
            aabb = new Rectangle(1,11, 13, 13);
            animPlayer = new AnimationPlayer();
            speed = 15f;
            awakeTime = 3f;
            awakeTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = -1f;
        }

        public override void Init()
        {
            texture = ResourceManager.Turtle;

            animPlayer.Add("walking", new Animation(texture, 0.2f, true, 16, 2, 0, 0));
            animPlayer.Add("stomped", new Animation(texture, 1.5f, false, 16, 1, 32, 0));
            animPlayer.Add("awaking", new Animation(texture, 0.25f, true, 16, 2, 32, 0));
            flip = SpriteEffects.None;
            CanKill = true;
        }


        public override void Hit()
        {
            if (currState == States.wandering)
            {
                currState = States.stomped;
                CanKill = false;
            }
            else if (currState == States.stomped)
            {
                currState = States.rebounding;
                CanKill = true;
            }
            else if (currState == States.rebounding)
            {
                currState = States.stomped;
                CanKill = false;
            }
            //CanCollide = false;
        }

        public override void Update(GameTime gameTime)
        {
            ApplyGravity();

            switch (currState)
            {
                case States.wandering:
                    {
                        animPlayer.PlayAnimation("walking");

                        vel.X = speed * elapsed * dir;

                        if(vel.X > 0)
                            flip = SpriteEffects.None;
                        else if(vel.X < 0)
                            flip = SpriteEffects.FlipHorizontally;

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
                        vel.X = 0;

                        awakeTimeAcc += elapsed;

                        if(awakeTimeAcc >= awakeTime)
                        {
                            awakeTimeAcc = 0f;
                            currState = States.wandering;
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
                        animPlayer.PlayAnimation("stomped");

                        vel.X = (speed * 20f * elapsed) *  dir;

                        if (RightWallHit)
                        {
                            dir = -1f;
                        }
                        else if (LeftWallHit)
                        {
                            dir = 1f;
                        }
                    }
                    break;
            }

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                flip);

            base.Draw(spriteBatch);
        }
    }
}
