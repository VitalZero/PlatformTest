﻿using Microsoft.Xna.Framework;
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

        public Vector2 Vel { get { return velocity; } }

        private const float jumpHoldTime = 0.25f;
        private float jumpTimer = 0;
        private KeyboardState keyboard;
        private SpriteEffects hFlip;
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

        public Player()
        {
            drawPriority = 1;
            position = new Vector2(50f, 50f);
            velocity = Vector2.Zero;
            dir = 1f;
            maxGravity = 2 * maxJumpHeight / (float)Math.Pow(timeToJumpPeak * 1.2, 2);
            normalGravity = 2 * jumpHeight / (float)Math.Pow(timeToJumpPeak, 2); // this is the normal one
            minGravity = 2 * minJumpHeight / (float)Math.Pow(timeToJumpPeak * .4, 2);
            jumpSpeed = normalGravity * timeToJumpPeak;
            bounceSpeed = maxGravity * timeToJumpPeak * .4f;
            gravity = normalGravity;
            pipeSpeed = 40f;

            speed = maxWalkSpeed;
            // rectangle coordinates for big mario, from the origin point (no texture origin)
            aabbBig = new Rectangle(-6, -24, 12, 24);
            // rectangle coordinates for small mario, from the origin point (no texture origin)
            aabbSmall = new Rectangle(-6, -12, 12, 12);
            animPlayer = new AnimationPlayer();
            power = Power.none;
            appended = "Small";
            aabb = aabbSmall;

            bounce = false;

            instance = this;

            currState = States.fall;
            prevState = currState;
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
            paleteSwap.Parameters["xTargetPal"].SetValue(pal1);
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
            velocity.Y = -bounceSpeed;
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

        // when mario changes sizes from small to big and viceversa
        // we also switch it's bounding box and pallete if applies
        private void Shrink()
        {
            origin = originSmall;
            aabb = aabbSmall;
            appended = "Small";
            paleteSwap.Parameters["xSourcePal"].SetValue(sourcePal);
            paleteSwap.Parameters["xTargetPal"].SetValue(pal1);
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

            PrevPos = position;

            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            // for debug purposes (switch to a transform)
            if (keyboard.IsKeyDown(Keys.G) && oldState.IsKeyUp(Keys.G))
            {
                Burn();
                power = Power.fire;
            }
            //if (keyboard.IsKeyDown(Keys.H) && oldState.IsKeyUp(Keys.H))
            //    Shrink();

            // delay the transform switching as changing the bounding box takes
            // effect inmediately and it looks weird (draw position and real position is not the same)
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
                        if((int)velocity.X >= 1 || (int)velocity.X <= -1)
                            animPlayer.PlayAnimation("running" + appended);
                        else
                            animPlayer.PlayAnimation("idle" + appended);

                        if (keyboard.IsKeyDown(Keys.A))
                        {
                            speed = maxRunSpeed;
                        }
                        else if (keyboard.IsKeyUp(Keys.A))
                        {
                            speed = maxWalkSpeed;
                        }

                        velocity.X = Lerp(velocity.X, 0, 0.15f);

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
                            animPlayer.PlayAnimation("jumping" + appended);
                            currState = States.jump;

                            velocity.Y = -jumpSpeed;  
                            jumpTimer = jumpHoldTime;
                            IsOnGround = false;
                            break;
                        }

                        if(keyboard.IsKeyDown(Keys.Down))
                        {
                            if (CheckForAreas() == AreaType.downPipe)
                            {
                                GoDownPipe();
                                break;
                            }

                            if (power > Power.none)
                            {
                                animPlayer.PlayAnimation("crouching");
                                currState = States.crouch;

                                break;
                            }
                        }

                        if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
                        {
                            if (EntityManager.FireBallCount < 2)
                            {
                                EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));

                                animPlayer.PlayAnimation("firing");
                                prevState = currState;
                                currState = States.firing;
                            }

                            break;
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
                            velocity.X = Lerp(velocity.X, speed, 0.05f);

                            hFlip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            dir = -1f;
                            velocity.X = Lerp(velocity.X, -speed, 0.05f);

                            hFlip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            animPlayer.PlayAnimation("jumping" + appended);
                            currState = States.jump;
                            velocity.Y = -jumpSpeed;
                            
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

                        if (keyboard.IsKeyDown(Keys.Down))
                        {
                            if (CheckForAreas() == AreaType.downPipe)
                            {
                                GoDownPipe();
                                break;
                            }

                            if (power > Power.none)
                            {
                                animPlayer.PlayAnimation("crouching");
                                currState = States.crouch;

                                break;
                            }
                        }

                        if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
                        {
                            if (EntityManager.FireBallCount < 2)
                            {
                                EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));

                                animPlayer.PlayAnimation("firing");
                                prevState = currState;
                                currState = States.firing;
                            }

                                break;
                        }
                    }
                    break;

                case States.jump:
                    {                        
                        if (keyboard.IsKeyDown(Keys.Right))
                        {
                            velocity.X = Lerp(velocity.X, speed, 0.05f);
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            velocity.X = Lerp(velocity.X, -speed, 0.05f);
                        }

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            velocity.X = Lerp(velocity.X, 0, 0.1f);
                        }

                        if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
                        {
                            if (EntityManager.FireBallCount < 2)
                                EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));
                        }

                        if (!bounce)
                        {
                            if (keyboard.IsKeyDown(Keys.S))
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

                        if (velocity.Y > 0)
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
                        velocity.X = Lerp(velocity.X, speed, 0.05f);
                    }
                    else if (keyboard.IsKeyDown(Keys.Left))
                    {
                        velocity.X = Lerp(velocity.X, -speed, 0.05f);
                    }

                    if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                    {
                        velocity.X = Lerp(velocity.X, 0, 0.1f);
                    }

                    if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
                    {
                        if (EntityManager.FireBallCount < 2)
                            EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));
                    }

                    if (IsOnGround)
                    {
                        if (animPlayer.CurrentAnimation() == "crouching")
                            aabb = aabbBig;

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
                        aabb = aabbSmall;

                        velocity.X = Lerp(velocity.X, 0, 0.15f);

                        if (!IsOnGround)
                        {
                            aabb = aabbBig;

                            currState = States.fall;
                            break;
                        }

                        if(keyboard.IsKeyUp(Keys.Down))
                        {
                            aabb = aabbBig;

                            currState = States.stand;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Right))
                        {
                            dir = 1f;

                            hFlip = SpriteEffects.None;
                        }
                        else if (keyboard.IsKeyDown(Keys.Left))
                        {
                            dir = -1f;

                            hFlip = SpriteEffects.FlipHorizontally;
                        }

                        if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
                        {
                            currState = States.jump;

                            velocity.Y = -jumpSpeed;
                            jumpTimer = jumpHoldTime;
                            IsOnGround = false;
                            break;
                        }
                    }
                    break;

                case States.downPipe:
                    {
                        velocity.Y = pipeSpeed;
                    }
                    break;
            }

            if (RightWallHit || LeftWallHit)
                speed = maxWalkSpeed;

            //vel.X = Math.Clamp(vel.X, -speed * elapsed, speed * elapsed);

            float updateSpeed = (currState == States.firing) ? 60 : Math.Max(50, Math.Abs(velocity.X)); // to change


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

        private AreaType CheckForAreas()
        {
            foreach (var a in World.Instance.GetTriggerAreas())
            {
                Rectangle pAABB = GetAABB();
                Rectangle aAABB = a.GetAABB();

                if (aAABB.Intersects(pAABB) && IsOnGround &&
                    pAABB.Right <= aAABB.Right && pAABB.Left >= aAABB.Left)
                {
                    return a.Type;
                }
            }

            return AreaType.none;
        }

        private void GoDownPipe()
        {
            CanCollide = false;
            DrawBehind = true;
            currState = States.downPipe;
            velocity = Vector2.Zero;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(effect: paleteSwap);

            animPlayer.Draw(spriteBatch,
                new Vector2((int)position.X - (int)Camera.Instance.XOffset, (int)position.Y - (int)Camera.Instance.YOffset),
                hFlip, origin);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred);

            base.Draw(spriteBatch);
        }

        // pretty basic "lerp"
        private float Lerp(float start, float end, float percent)
        {
            return (start * (1f - percent)) + (end * percent);
        }

        private float MapValue(float maxValue, float minValue, float knowMaxValue)
        {
            return (minValue * knowMaxValue) / maxValue;
        }
    }
}

