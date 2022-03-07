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
            this.pos = pos;
            aabb = new Rectangle(2, 3, 12, 13);
            animPlayer = new AnimationPlayer();
            speed = 20f;
            deadTime = 0.5f;
            deadTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = -1f;
            Active = false;
            vFlip = SpriteEffects.None;
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
            CanKill = false;
            CanCollide = false;
        }

        public override void Kill()
        {
            currState = States.instantKill;
            vel.Y = -250f;
            CanKill = false;
            CanCollide = false;
            speed = 30f;
            vFlip = SpriteEffects.FlipVertically;
            isOnGround = false;
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
                        vel.X = 0f;

                        animPlayer.PlayAnimation("stomped");

                        if (deadTimeAcc >= deadTime)
                        {
                            Active = false;
                            Destroyed = true;
                        }
                    }
                    break;
                case States.instantKill:
                    {
                        animPlayer.Freeze();
                        deadTimeAcc += elapsed;

                        vel.X = speed * dir;

                        if (deadTimeAcc >= 1f)
                        {
                            Active = false;
                            Destroyed = true;
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
                vFlip);

            base.Draw(spriteBatch);
        }
    }
}
