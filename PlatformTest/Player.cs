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
        private enum States { stand, run, jump, fall, crouch }

        public Vector2 Vel { get { return vel; } }

        private Vector2 origin;


        private const float jumpHoldTime = 0.25f;
        private float jumpTimer = 0;
        private KeyboardState keyboard;
        private SpriteEffects flip;
        private AnimationPlayer animPlayer;
        private States currState;
        public bool Pause { get; set; }
        private const float maxWalkSpeed = 60f;
        private const float maxRunSpeed = 170f;
        private const float moveXAccel = 4f;
        private const float stopAccel = 6f;
        public Vector2 PrevPos { get; private set; }
        bool bounce;

        protected float jumpSpeed;
        protected float minGravity;
        protected float normalGravity;
        protected float maxGravity;
        protected float bounceSpeed;
        readonly float timeToJumpPeak = 0.4f;
        readonly int jumpHeight = (int)(4.25 * 16);
        readonly int minJumpHeight = (int)(2 * 16);
        readonly int maxJumpHeight = (int)(5.2 * 16);

        Effect paleteSwap;
        Texture2D sourcePal;
        Texture2D pal1;
        Texture2D pal2;

        //for debug purposes
        List<string> playerStates = new List<string>();
        SpriteFont font;
        //

        public Player()
        {
            drawPriority = 1;
            pos = new Vector2(50f, 50f);
            size = new Vector2(16, 31);
            vel = Vector2.Zero;
            dir = 0f;
            //jumpSpeed = -240f;
            //gravity = 24f;
            maxGravity = 2 * maxJumpHeight / (float)Math.Pow(timeToJumpPeak * 1.2, 2);
            normalGravity = 2 * jumpHeight / (float)Math.Pow(timeToJumpPeak, 2); // this is the normal one
            minGravity = 2 * minJumpHeight / (float)Math.Pow(timeToJumpPeak * .4, 2);
            jumpSpeed = normalGravity * timeToJumpPeak;
            bounceSpeed = maxGravity * timeToJumpPeak * .4f;
            gravity = normalGravity;

            //gravity *= 0.0167f; // if I dont do this, doesnt jump

            speed = maxWalkSpeed;
            aabb = new Rectangle(2, 4, 12, 27);
            origin = new Vector2(size.X / 2, size.Y);
            animPlayer = new AnimationPlayer();
            Pause = false;

            bounce = false;

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
            dir = 1f;

            texture = ResourceManager.Player;
            paleteSwap = ResourceManager.ColorSwap;
            sourcePal = ResourceManager.SourcePal;
            pal1 = ResourceManager.Pal1;
            pal2 = ResourceManager.Pal2;

            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(sourcePal);
            paleteSwap.CurrentTechnique.Passes[0].Apply();

            animPlayer.Add("idle", new Animation(texture, 1f, true, 16, 32, 1, 0, 0));
            animPlayer.Add("running", new Animation(texture, .04f, true, 16, 32, 3, 16, 0));
            animPlayer.Add("jumping", new Animation(texture, .1f, true, 16, 32, 1, 16 * 6, 0));
            animPlayer.Add("falling", new Animation(texture, 1f, true, 16, 32, 1, 16 * 5, 0));
            animPlayer.Add("crouching", new Animation(texture, 1f, true, 16, 32, 1, 16 * 7, 0));
            animPlayer.PlayAnimation("idle");
            font = ResourceManager.Arial;
        }

        public void Bounce()
        {
            bounce = true;
            currState = States.jump;
            vel.Y = -bounceSpeed;
            //gravity = minGravity;
        }

        public override void Hit()
        {
            CanCollide = false;
            Active = false;
            Destroyed = true;
        }

        public void Burn()
        {
            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(pal2);
            paleteSwap.CurrentTechnique.Passes[0].Apply();
        }

        public void Grow()
        {
            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(pal1);
            paleteSwap.CurrentTechnique.Passes[0].Apply();
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                            vel.Y = -jumpSpeed;  
                            jumpTimer = jumpHoldTime;
                            isOnGround = false;
                            break;
                        }

                        if(keyboard.IsKeyDown(Keys.Down))
                        {
                            currState = States.crouch;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A))
                        {
                            if(EntityManager.FireBallCount() < 2)
                                EntityManager.Add(new FireBall(pos, dir));
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
                            dir = 1f;
                            //vel.X = speed;

                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            dir = -1f;
                            //vel.X = -speed;

                            flip = SpriteEffects.FlipHorizontally;
                        }

                        vel.X = speed * dir;

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;
                            vel.Y = -jumpSpeed;
                            
                            if(speed == maxRunSpeed)
                                gravity = maxGravity;

                            jumpTimer = jumpHoldTime; 
                            isOnGround = false;
                            break;
                        }
                        else if (!isOnGround)
                        {
                            currState = States.fall;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Down))
                        {
                            currState = States.crouch;
                            vel.X = 0;
                            break;
                        }
                    }
                    break;

                case States.jump:
                    {
                        animPlayer.PlayAnimation("jumping");

                        
                        if (keyboard.IsKeyDown(Keys.Right))
                        {
                            dir = 1f;
                            //vel.X = speed;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            dir = -1f;
                            //vel.X = speed;
                        }

                        vel.X = speed * dir;

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = 0;
                        }

                        if (!bounce)
                        {
                            if (keyboard.IsKeyDown(Keys.S))// || jumpTimer < ((jumpHoldTime / 3)))
                            {
                                if ((int)gravity == (int)maxGravity)
                                    gravity = maxGravity;
                                else
                                    gravity = normalGravity;
                            }
                            else if (keyboard.IsKeyUp(Keys.S))
                            {
                                gravity = minGravity;
                            }
                        }
                        else
                        {
                            gravity = maxGravity;
                        }

                        jumpTimer -= elapsed;

                        if (vel.Y > 0)
                        {
                            currState = States.fall;
                            bounce = false;
                            break;
                        }
                    }
                    break;

                case States.fall:
                    gravity = normalGravity;
                    animPlayer.Freeze();

                    if (keyboard.IsKeyDown(Keys.Right))
                    {
                        dir = 1f;
                        //vel.X = speed;
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        dir = -1f;
                        //vel.X = -speed;
                    }

                    vel.X = speed * dir;

                    if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                    {
                        vel.X = 0;
                    }

                    if (isOnGround)
                    {
                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                        }
                        else
                        {
                            currState = States.run;
                        }
                    }
                    break;

                case States.crouch:
                    {
                        animPlayer.PlayAnimation("crouching");

                        if (!isOnGround)
                        {
                            currState = States.fall;
                            break;
                        }

                        if(keyboard.IsKeyUp(Keys.Down))
                        {
                            currState = States.stand;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Right))
                        {
                            dir = 1f;

                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            dir = -1f;

                            flip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;

                            vel.Y = -jumpSpeed;
                            jumpTimer = jumpHoldTime;
                            isOnGround = false;
                            break;
                        }
                    }
                    break;
            }

            if (RightWallHit || LeftWallHit)
                speed = maxWalkSpeed;

            //vel.X = Math.Clamp(vel.X, -speed * elapsed, speed * elapsed);

            animPlayer.Update(MapValue(maxRunSpeed, Math.Abs(vel.X), elapsed));

            LateUpdate(gameTime);


            if (CeilingHit)
            {
                Point tilePos = GetContactTile();
                Tile t = World.Instance.GetTile(tilePos.X, tilePos.Y);

                if (t.collision == TileCollision.breakable || t.collision == TileCollision.item)
                    World.Instance.RemoveTile(tilePos.X, tilePos.Y);
            }

            //dir = 0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(effect: paleteSwap);

            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                flip);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred);

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

        private float MapValue(float maxValue, float minValue, float knowMaxValue)
        {
            return (minValue * knowMaxValue) / maxValue;
        }
    }
}

