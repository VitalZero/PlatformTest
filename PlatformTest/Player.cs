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
        private enum States { stand, run, jump, fall, crouch, firing, downPipe, upPipe }
        private enum Power { none, big, fire }

        public Vector2 Vel { get { return vel; } }

        private const float jumpHoldTime = 0.25f;
        private float jumpTimer = 0;
        private KeyboardState keyboard;
        private SpriteEffects flip;
        private AnimationPlayer animPlayer;
        private States currState;
        private States prevState;
        private Power power;
        private const float maxWalkSpeed = 60f;
        private const float maxRunSpeed = 170f;
        private const float moveXAccel = 4f;
        private const float stopAccel = 6f;
        public Vector2 PrevPos { get; private set; }
        private bool bounce;
        private float pipeSpeed;
        private bool canTransform = false;

        protected float jumpSpeed;
        protected float minGravity;
        protected float normalGravity;
        protected float maxGravity;
        protected float bounceSpeed;
        readonly float timeToJumpPeak = 0.4f;
        readonly int jumpHeight = (int)(4.25 * 16);
        readonly int minJumpHeight = (int)(2 * 16);
        readonly int maxJumpHeight = (int)(5.2 * 16);
        private string appended;
        private Rectangle aabbSmall;
        private Rectangle aabbBig;
        private readonly Vector2 originSmall = new Vector2(8, 15);
        private readonly Vector2 originBig = new Vector2(8, 31);

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
            vel = Vector2.Zero;
            dir = 1f;
            maxGravity = 2 * maxJumpHeight / (float)Math.Pow(timeToJumpPeak * 1.2, 2);
            normalGravity = 2 * jumpHeight / (float)Math.Pow(timeToJumpPeak, 2); // this is the normal one
            minGravity = 2 * minJumpHeight / (float)Math.Pow(timeToJumpPeak * .4, 2);
            jumpSpeed = normalGravity * timeToJumpPeak;
            bounceSpeed = maxGravity * timeToJumpPeak * .4f;
            gravity = normalGravity;
            pipeSpeed = 30f;

            speed = maxWalkSpeed;
            //aabbBig = new Rectangle(2, 4, 12, 27);
            //aabbSmall = new Rectangle(2, 3, 12, 12);
            aabbBig = new Rectangle(-6, -27, 12, 27);
            aabbSmall = new Rectangle(-6, -12, 12, 12);
            animPlayer = new AnimationPlayer();
            power = Power.none;
            appended = "Small";
            aabb = aabbSmall;

            bounce = false;

            instance = this;

            currState = States.fall;
            prevState = currState;

            //pos -= origin;
        }

        public override void Init()
        {
            origin = originSmall;

            texture = ResourceManager.Player;
            paleteSwap = ResourceManager.ColorSwap;
            sourcePal = ResourceManager.SourcePal;
            pal1 = ResourceManager.Pal1;
            pal2 = ResourceManager.Pal2;

            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(sourcePal);
            paleteSwap.CurrentTechnique.Passes[0].Apply();

            animPlayer.Add("idleSmall", new Animation(texture, 1f, true, 16, 16, 1, 0, 16 * 2));
            animPlayer.Add("runningSmall", new Animation(texture, .04f, true, 16, 32, 3, 16, 16 * 2));
            animPlayer.Add("jumpingSmall", new Animation(texture, .1f, true, 16, 32, 1, 16 * 6, 16 * 2));
            animPlayer.Add("skidSmall", new Animation(texture, 1f, true, 16, 32, 1, 16 * 5, 16 * 2));
            animPlayer.Add("killed", new Animation(texture, 1f, true, 16, 32, 1, 16 * 4, 16 * 2));

            animPlayer.Add("idleBig", new Animation(texture, 1f, true, 16, 32, 1, 0, 0));
            animPlayer.Add("runningBig", new Animation(texture, .04f, true, 16, 32, 3, 16, 0));
            animPlayer.Add("jumpingBig", new Animation(texture, .1f, true, 16, 32, 1, 16 * 6, 0));
            animPlayer.Add("skidBig", new Animation(texture, 1f, true, 16, 32, 1, 16 * 5, 0));

            animPlayer.Add("crouching", new Animation(texture, 1f, true, 16, 32, 1, 16 * 7, 0));
            animPlayer.Add("firing", new Animation(texture, 0.04f, false, 16, 32, 1, 16 * 4, 0));

            animPlayer.PlayAnimation("idle" + appended);
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
            if (power > Power.none)
            {
                power = Power.none;
                canTransform = true;
            }
            else
            {
                CanCollide = false;
                //Active = false;
                //Destroyed = true;
            }
        }

        public void CollectedPowerUp()
        {
            if (power == Power.none)
            {
                power = Power.big;
                canTransform = true;
            }
            else if (power == Power.big)
            {
                power = Power.fire;
                canTransform = true;
            }
        }

        private void Shrink()
        {
            origin = originSmall;
            aabb = aabbSmall;
            appended = "Small";
            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(sourcePal);
            paleteSwap.CurrentTechnique.Passes[0].Apply();
        }

        private void Burn()
        {
            origin = originBig;
            aabb = aabbBig;
            appended = "Big";
            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(pal2);
            paleteSwap.CurrentTechnique.Passes[0].Apply();
        }

        private void Grow()
        {
            origin = originBig;
            aabb = aabbBig;
            appended = "Big";
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

            if (keyboard.IsKeyDown(Keys.G) && oldState.IsKeyUp(Keys.G))
            {
                Burn();
                power = Power.fire;
            }
            //if (keyboard.IsKeyDown(Keys.H) && oldState.IsKeyUp(Keys.H))
            //    Shrink();

            if(canTransform)
            {
                if (power == Power.none)
                    Shrink();
                if (power == Power.big)
                    Grow();
                if (power == Power.fire)
                    Burn();
            }

            switch (currState)
            {
                case States.stand:
                    {
                        animPlayer.PlayAnimation("idle" + appended);

                        if (keyboard.IsKeyDown(Keys.A))
                        {
                            speed = maxRunSpeed;
                        }
                        else if (keyboard.IsKeyUp(Keys.A))
                        {
                            speed = maxWalkSpeed;
                        }

                        vel.X = 0;

                        if (!IsOnGround)
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
                            IsOnGround = false;
                            break;
                        }

                        if(keyboard.IsKeyDown(Keys.Down) && power > Power.none)
                        {
                            animPlayer.PlayAnimation("crouching");
                            currState = States.crouch;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
                        {
                            animPlayer.PlayAnimation("firing");
                            prevState = currState;
                            currState = States.firing;
                            if(EntityManager.FireBallCount < 2)
                                EntityManager.Add(new FireBall(new Vector2(flip == 0 ? pos.X : pos.X - 7, pos.Y - 28), flip == 0 ? 1f : -1f));
                        }
                    }
                    break;

                case States.run:
                    {
                        animPlayer.PlayAnimation("running" + appended);

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
                            vel.X = speed;

                            flip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            dir = -1f;
                            vel.X = -speed;

                            flip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;
                            vel.Y = -jumpSpeed;
                            
                            if(speed == maxRunSpeed)
                                gravity = maxGravity;

                            jumpTimer = jumpHoldTime; 
                            IsOnGround = false;
                            break;
                        }
                        else if (!IsOnGround)
                        {
                            currState = States.fall;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Down) && power > Power.none)
                        {
                            animPlayer.PlayAnimation("crouching");
                            currState = States.crouch;
                            vel.X = 0;
                            break;
                        }
                    }
                    break;

                case States.jump:
                    {
                        animPlayer.PlayAnimation("jumping" + appended);

                        
                        if (keyboard.IsKeyDown(Keys.Right))
                        {
                            vel.X = speed;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = -speed;
                        }

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
                        vel.X = speed;
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        vel.X = -speed;
                    }

                    if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                    {
                        vel.X = 0;
                    }

                    if (IsOnGround)
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

                case States.firing:
                    {
                        if(animPlayer.AnimationEnded("firing"))
                        {
                            currState = prevState;
                        }
                    }
                    break;

                case States.crouch:
                    {
                        if (keyboard.IsKeyDown(Keys.Down))
                        {
                            if (CheckForAreas())
                                break;
                        }

                        if (!IsOnGround)
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
                            IsOnGround = false;
                            break;
                        }
                    }
                    break;

                case States.downPipe:
                    {
                        vel.Y = pipeSpeed;
                    }
                    break;
            }

            if (RightWallHit || LeftWallHit)
                speed = maxWalkSpeed;

            //vel.X = Math.Clamp(vel.X, -speed * elapsed, speed * elapsed);

            float updateSpeed = (currState == States.firing) ? 60 : Math.Abs(vel.X); // to change

            animPlayer.Update(MapValue(maxRunSpeed, updateSpeed, elapsed));

            LateUpdate(gameTime);


            if (CeilingHit)
            {
                Point tilePos = GetContactTile();
                Tile t = World.Instance.GetTile(tilePos.X, tilePos.Y);

                if (power > Power.none)
                {
                    if (t.collision == TileCollision.breakable || t.collision == TileCollision.item)
                        World.Instance.RemoveTile(tilePos.X, tilePos.Y);
                }
                else
                {
                    if (t.collision == TileCollision.item)
                        World.Instance.RemoveTile(tilePos.X, tilePos.Y);
                }
            }
            //dir = 0f;
        }

        private bool CheckForAreas()
        {
            foreach (var a in World.Instance.GetTriggerAreas())
            {
                Rectangle pAABB = GetAABB();
                Rectangle aAABB = a.GetAABB();

                if (aAABB.Intersects(pAABB) && IsOnGround &&
                    pAABB.Right <= aAABB.Right && pAABB.Left >= aAABB.Left)
                {
                    if(a.Type == AreaType.downPipe)
                        GoDownPipe();
                    return true;
                }
            }

            return false;
        }

        private void GoDownPipe()
        {
            CanCollide = false;
            DrawBehind = true;
            currState = States.downPipe;
            vel = Vector2.Zero;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(effect: paleteSwap);

            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                flip, origin);

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

