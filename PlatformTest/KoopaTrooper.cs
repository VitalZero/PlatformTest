﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class KoopaTrooper : Enemy
    {
        private enum States { wandering, stomped, rebounding }

        private AnimationPlayer animPlayer;
        private float awakeTime;
        private float awakeTimeAcc;
        private States currState;
        private SpriteEffects flip;

        public KoopaTrooper(Vector2 pos, int index)
        {
            Index = index;
            this.pos = pos;
            aabb = new Rectangle(1,11, 13, 13);
            animPlayer = new AnimationPlayer();
            speed = 20f;
            awakeTime = 3f;
            awakeTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = -1f;
            Active = false;
        }

        public override void Init()
        {
            texture = ResourceManager.Turtle;

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
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (currState)
            {
                case States.wandering:
                    {
                        animPlayer.PlayAnimation("walking");

                        vel.X = speed * dir;

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

                        vel.X = (speed * 15f) *  dir;

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

            animPlayer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

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
