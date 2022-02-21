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
        private KeyboardState keyboard;
        private SpriteEffects flip;
        private AnimationPlayer animPlayer;
        private States currState;
        private const float jumpHoldTime = 0.4f; //0.25f;
        private float localHoldTime = 0;
        public bool Pause { get; set; }
        private const float maxWalkSpeed = 90f;
        private const float maxRunSpeed = 150;
        private const float moveXAccel = 3.26f;
        private const float stopAccel = 3.26f;

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
            jumpSpeed = -180f; //-220f;
            speed = maxRunSpeed;
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

            animPlayer.Add("idle", new Animation(texture, 1f, true, 16, 1, 0, 0));
            animPlayer.Add("running", new Animation(texture, .04f, true, 16, 4, 16, 0));
            animPlayer.Add("jumping", new Animation(texture, .1f, true, 16, 1, 16 * 6, 0));
            animPlayer.Add("falling", new Animation(texture, 1f, true, 16, 1, 16 * 5, 0));
            animPlayer.PlayAnimation("idle");
            font = ResourceManager.Arial;
        }

        public void Bounce()
        {
            vel.Y = jumpSpeed * elapsed;
            currState = States.jump;
        }

        public override void Hit()
        {
            CanCollide = false;
            Active = false;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A))
                Pause = !Pause;

            if (Pause)
                return;

            switch (currState)
            {
                case States.stand:
                    {
                        animPlayer.PlayAnimation("idle");

                        vel.X = 0;

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
                            vel.Y = jumpSpeed * elapsed;
                            localHoldTime = jumpHoldTime;
                            isOnGround = false;
                            break;
                        }
                    }
                    break;

                case States.run:
                    {
                        animPlayer.PlayAnimation("running");

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X = speed * elapsed;

                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = -speed * elapsed;

                            flip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;
                            vel.Y = jumpSpeed * elapsed;
                            localHoldTime = jumpHoldTime;
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
                            vel.X = 0;
                        }
                        else if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X = speed * elapsed;
                            //flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = -speed * elapsed;
                            //flip = SpriteEffects.FlipHorizontally;
                        }
                        if (keyboard.IsKeyDown(Keys.S))
                        {
                            if (localHoldTime > 0)
                                vel.Y = jumpSpeed * elapsed;
                            else
                                localHoldTime = 0;

                            if (CeilingHit)
                            {
                                localHoldTime = 0;
                                vel.Y = 0;
                            }
                        }

                        localHoldTime -= elapsed;

                        if (isOnGround)
                        {
                            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                            {
                                currState = States.stand;
                                vel = Vector2.Zero;
                                break;
                            }
                            else
                            {
                                currState = States.run;
                                vel.Y = 0f;
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
                        vel.X = 0;
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        vel.X = speed * elapsed;
                        //flip = SpriteEffects.None;
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        vel.X = -speed * elapsed;
                        //flip = SpriteEffects.FlipHorizontally;
                    }

                    if (isOnGround)
                    {
                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            vel = Vector2.Zero;
                        }
                        else
                        {
                            currState = States.run;
                            vel.Y = 0f;
                        }
                    }

                    break;
            }

            ApplyGravity();

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);

            if (CeilingHit)
            {
                //Pause = true;
                Point tilePos = GetContactTile();
                Tile t = World.Instance.GetTile(tilePos.X, tilePos.Y);

                if (t.collision == TileCollision.breakable)
                    World.Instance.RemoveTile(tilePos.X, tilePos.Y);
                else if (t.collision == TileCollision.item)
                    World.Instance.usedTileItem(tilePos.X, tilePos.Y);
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
