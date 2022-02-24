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
        private enum States { wandering, stomped }

        private AnimationPlayer animPlayer;
        private float deadTime;
        private float deadTimeAcc;
        private States currState;

        public Goomba(Vector2 pos)
        {
            this.pos = pos;
            aabb = new Rectangle(2, 3, 12, 13);
            animPlayer = new AnimationPlayer();
            speed = 15f;
            deadTime = 0.5f;
            deadTimeAcc = 0;
            currState = States.wandering;
            CanKill = true;
            dir = -1f;
        }

        public override void Init()
        {
            texture = ResourceManager.Goomba;

            animPlayer.Add("walking", new Animation(texture, 0.2f, true, 16, 2, 0, 0));
            animPlayer.Add("stomped", new Animation(texture, 1f, false, 16, 1, 32, 0));
        }

        public override void Hit()
        {
            currState = States.stomped;
            CanKill = false;
            CanCollide = false;
        }

        public override void Update(GameTime gameTime)
        {
            //ApplyGravity();

            switch (currState)
            {
                case States.wandering:
                    {
                        animPlayer.PlayAnimation("walking");
                        vel.X = speed * elapsed * dir;

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
            }

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                SpriteEffects.None);

            base.Draw(spriteBatch);
        }
    }
}
