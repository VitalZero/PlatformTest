using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace PlatformTest
{
    public class Player : Entity
    {
        private static Player instance = null;
        public static Player Instance
        {
            get
            {
                if (instance == null)
                    instance = new Player();

                return instance;
            }
        }
        private enum States { stand, run, jump, fall }

        public Vector2 Vel { get { return vel; } }

        private Vector2 origin;
        protected float jumpSpeed;
        private const float jumpHoldTime = 0.30f;
        private float jumpTimer = 0;
        private KeyboardState keyboard;
        private SpriteEffects flip;
        private AnimationPlayer animPlayer;
        private States currState;
        public bool Pause { get; set; }
        private const float maxWalkSpeed = 90f;
        private const float maxRunSpeed = 164f;
        private const float moveXAccel = 4f;
        private const float stopAccel = 5f;
        public Vector2 PrevPos { get; private set; }

        //for debug purposes
        List<string> playerStates = new List<string>();
        SpriteFont font;
        //

        public Player()
        {
            pos = new Vector2(50f, 50f);
            size = new Vector2(16, 31);
            vel = Vector2.Zero;
            dir = 0f;
            jumpSpeed = -200f;
            gravity = 20f;
            speed = maxWalkSpeed;
            aabb = new Rectangle(2, 4, 12, 27);
            origin = new Vector2(size.X / 2, size.Y);
            animPlayer = new AnimationPlayer();
            Pause = false;

            instance = this;

            currState = States.fall;

            //for debug
            playerStates.Add("STAND");
            playerStates.Add("RUN");
            playerStates.Add("JUMP");
            playerStates.Add("FALL");
            //
        }

        public override void Init()
        {
            texture = ResourceManager.Player;

            animPlayer.Add("idle", new Animation(texture, 1f, true, 16, 32, 1, 0, 0));
            animPlayer.Add("running", new Animation(texture, .04f, true, 16, 32, 4, 16, 0));
            animPlayer.Add("jumping", new Animation(texture, .1f, true, 16, 32, 1, 16 * 6, 0));
            animPlayer.Add("falling", new Animation(texture, 1f, true, 16, 32, 1, 16 * 5, 0));
            animPlayer.PlayAnimation("idle");
            font = ResourceManager.Arial;
        }

        public void Bounce()
        {
            vel.Y = jumpSpeed * elapsed;
            currState = States.jump;
            jumpTimer = jumpHoldTime;
        }

        public override void Hit()
        {
            CanCollide = false;
            Active = false;
            Destroyed = true;
        }

        public override void Update(GameTime gameTime)
        {
            PrevPos = pos;

            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W) && oldState.IsKeyUp(Keys.W))
                Pause = !Pause;

            if (Pause)
                return;

            switch (currState)
            {
                case States.stand:
                    {
                        animPlayer.PlayAnimation("idle");

                        if (keyboard.IsKeyDown(Keys.A))
                        {
                            speed = maxRunSpeed;
                        }
                        else if (keyboard.IsKeyUp(Keys.A))
                        {
                            speed = maxWalkSpeed;
                        }

                        if (vel.X < 0)
                        {
                            vel.X += stopAccel * elapsed;
                            if (vel.X >= 0)
                                vel.X = (int)0f;
                        }
                        else if (vel.X > 0)
                        {
                            vel.X += -stopAccel * elapsed;
                            if (vel.X <= 0)
                                vel.X = (int)0f;
                        }

                        //vel.X = 0;

                        if (!isOnGround)
                        {
                            currState = States.fall;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Right) != keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.run;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;
                            //vel.Y = jumpSpeed * elapsed;
                            jumpTimer = jumpHoldTime;
                            isOnGround = false;
                            break;
                        }
                    }
                    break;

                case States.run:
                    {
                        animPlayer.PlayAnimation("running");

                        if(keyboard.IsKeyDown(Keys.A))
                        {
                            speed = maxRunSpeed;
                        }
                        else if(keyboard.IsKeyUp(Keys.A))
                        {
                            speed = maxWalkSpeed;
                        }

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X += moveXAccel * elapsed;
                            //vel.X = speed * elapsed;

                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X += -moveXAccel * elapsed;
                            //vel.X = -speed * elapsed;

                            flip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;
                            //vel.Y = jumpSpeed * elapsed;
                            if (speed == maxRunSpeed )
                                jumpTimer = .40f;
                            else
                                jumpTimer = jumpHoldTime; 
                            isOnGround = false;
                            break;
                        }
                        else if (!isOnGround)
                        {
                            currState = States.fall;
                            break;
                        }
                    }
                    break;

                case States.jump:
                    {
                        animPlayer.PlayAnimation("jumping");

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            if (vel.X < 0)
                            {
                                vel.X += stopAccel * elapsed;
                                if (vel.X >= 0)
                                    vel.X = (int)0f;
                            }
                            else if (vel.X > 0)
                            {
                                vel.X += -stopAccel * elapsed;
                                if (vel.X <= 0)
                                    vel.X = (int)0f;
                            }

                            //vel.X = 0;
                        }
                        else if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X += moveXAccel * elapsed;
                            //vel.X = speed * elapsed;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X += -moveXAccel * elapsed;
                            //vel.X = -speed * elapsed;
                        }
                        if (keyboard.IsKeyDown(Keys.S))
                        {
                            if (jumpTimer > 0)
                                vel.Y = jumpSpeed * elapsed;
                            else
                                jumpTimer = 0;

                            if (CeilingHit)
                            {
                                jumpTimer = 0;
                                vel.Y = (int)0;
                            }
                        }
                        else if(!keyboard.IsKeyDown(Keys.S))
                        {
                            jumpTimer = 0;
                        }

                        jumpTimer -= elapsed;

                        if (isOnGround)
                        {
                            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                            {
                                currState = States.stand;
                                //vel = Vector2.Zero;
                                vel.Y = (int)0f;
                                break;
                            }
                            else
                            {
                                currState = States.run;
                                vel.Y = (int)0f;
                                break;
                            }
                        }

                        if(vel.Y > 0)
                        {
                            currState = States.fall;
                            break;
                        }
                    }
                    break;

                case States.fall:
                    animPlayer.Freeze();
                    //animPlayer.PlayAnimation(fall);
                    if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                    {
                        if (vel.X < 0)
                        {
                            vel.X += stopAccel * elapsed;
                            if (vel.X >= 0)
                                vel.X = (int)0f;
                        }
                        else if (vel.X > 0)
                        {
                            vel.X += -stopAccel * elapsed;
                            if (vel.X <= 0)
                                vel.X = (int)0f;
                        }

                        //vel.X = 0;
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        vel.X += moveXAccel * elapsed;
                        //vel.X = speed * elapsed;
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        vel.X += -moveXAccel * elapsed;
                        //vel.X = -speed * elapsed;
                    }

                    if (isOnGround)
                    {
                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            //vel = Vector2.Zero;
                            vel.Y = (int)0f;
                        }
                        else
                        {
                            currState = States.run;
                            vel.Y = (int)0f;
                        }
                    }

                    break;
            }

            if (RightWallHit || LeftWallHit)
                speed = maxWalkSpeed;

            vel.X = Math.Clamp(vel.X, -speed * elapsed, speed * elapsed);

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);


            if (CeilingHit)
            {
                Point tilePos = GetContactTile();
                Tile t = World.Instance.GetTile(tilePos.X, tilePos.Y);

                if (t.collision == TileCollision.breakable)
                    World.Instance.RemoveTile(tilePos.X, tilePos.Y);
                else if (t.collision == TileCollision.item)
                {
                    World.Instance.usedTileItem(tilePos.X, tilePos.Y);
                }
            }

            dir = 0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                flip);


            base.Draw(spriteBatch);
        }

        private float Lerp(float startValue, float endValue, float amount)
        {
            //return (startValue * (1f - amount)) + (endValue * amount);
            //return startValue + amount * (endValue - startValue);
            if (startValue >= endValue)
                return endValue;

            return startValue + amount;
        }
    }
}
