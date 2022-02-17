using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Turtle : Entity
    {
        private enum States { wandering, stomped, rebounding }

        private Animation walking;
        private Animation stomped;
        private Animation awaking;
        private AnimationPlayer animPlayer;
        private float deadTime;
        private float deadTimeAcc;
        private States currState;
        private SpriteEffects flip;
        public bool CanKill { get; set; }
        float dir;

        public Turtle()
        {
            pos = new Vector2(364, 50);
            aabb = new Rectangle(2, 8, 14, 16);
            animPlayer = new AnimationPlayer();
            speed = 20f;
            deadTime = 1f;
            deadTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = 1f;
        }

        public override void Init()
        {
            texture = ResourceManager.Turtle;

            walking = new Animation(texture, 0.2f, true, 16, 2, 0, 0);
            stomped = new Animation(texture, 1f, false, 16, 1, 32, 0);
            flip = SpriteEffects.None;
        }

        public override void Hit()
        {
            if (currState == States.wandering)
            {
                currState = States.stomped;
            }
            else if (currState == States.stomped)
            {
                currState = States.rebounding;
            }
            else if (currState == States.rebounding)
            {
                currState = States.stomped;
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
                        CanKill = true;
                        animPlayer.PlayAnimation(walking);

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
                        CanKill = false;

                        animPlayer.PlayAnimation(stomped);
                        vel.X = 0;
                    }
                    break;
                case States.rebounding:
                    {
                        CanKill = true;

                        animPlayer.PlayAnimation(stomped);

                        vel.X = (speed * 10f) * elapsed * dir;

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
        }
    }
}
