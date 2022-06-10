using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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

        private enum States { stand, run, jump, fall, crouch, firing, growing, shrinking, burning, downPipe, rightPipe, leftPipe, upPipe, goal }
        private enum Power { none, big, fire}

        public Vector2 PrevPos { get; private set; }
        public bool IsInvencible { get; set; }
        public Vector2 Vel { get { return velocity; } }
        public bool IsTransforming { get; private set; }

        private float invencibleTimer;
        private const float invencibleTotalTime = 3f;
        private const float starTotalTime = 10f;
        private float starTimer;
        public bool HasStar { get; private set; }
        private bool isBurning;
        private float secondCounter;
        private const float transformationTotalTime = 1f;
        private const float jumpHoldTime = 0.25f;
        private KeyboardState keyboard;
        KeyboardState oldState;
        private SpriteEffects hFlip;
        private AnimationPlayer animPlayer;
        private States currState;
        private States prevState;
        private Power power;
        private const float maxWalkSpeed = 60f;
        private const float maxRunSpeed = 170f;
        private bool bounce;
        private float pipeSpeed;
        private bool canTransform = false;

        protected float jumpSpeed;
        protected float minGravity;
        protected float normalGravity;
        protected float maxGravity;
        protected float bounceSpeed;
        readonly float timeToJumpPeak = 0.44f;
        readonly int jumpHeight = (int)(4.25 * 16);
        readonly int minJumpHeight = (int)(2 * 16);
        readonly int maxJumpHeight = (int)(5.2 * 16);
        private string appended;
        private Rectangle aabbSmall;
        private Rectangle aabbBig;
        private readonly Vector2 originSmall = new Vector2(8, 15);
        private readonly Vector2 originBig = new Vector2(8, 31);
        private Vector2 prevVelocity;
        Color color;

        Color[] starColor;
        private int colorStep = 0;

        // delegates
        public event Action deathEvent;

        private RenderTarget2D target;

        //for debug purposes
        List<string> playerStates = new List<string>();
        SpriteFont font;

        public Player()
        {
            drawPriority = 1;
            position = new Vector2(50f, 50f);
            velocity = Vector2.Zero;
            prevVelocity = Vector2.Zero;
            dir = 1f;
            maxGravity = 2 * maxJumpHeight / (float)Math.Pow(timeToJumpPeak * 1.2, 2);
            normalGravity = 2 * jumpHeight / (float)Math.Pow(timeToJumpPeak, 2); // this is the normal one
            minGravity = 2 * minJumpHeight / (float)Math.Pow(timeToJumpPeak * .4, 2);
            jumpSpeed = normalGravity * timeToJumpPeak;
            bounceSpeed = maxGravity * timeToJumpPeak * .4f;
            gravity = normalGravity;
            pipeSpeed = 40f;
            IsInvencible = false;
            invencibleTimer = 0f;
            starTimer = 0f;
            IsTransforming = false;

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
            isBurning = false;

            currState = States.fall;
            prevState = currState;

            starColor = new Color[] { Color.Red, Color.DarkGreen, new Color(100, 100, 10), Color.Blue, Color.White };
        }

        public override void Init()
        {
            origin = originSmall;

            texture = TextureManager.Player;

            AddAnimations();

            animPlayer.PlayAnimation("idle" + appended);
            font = TextureManager.Arial;

        }

        private void AddAnimations()
        {
            animPlayer.Add("idleSmall", new Animation(1f, true, 16, 16, 1, 0, 16 * 2));
            animPlayer.Add("runningSmall", new Animation(.04f, true, 16, 16, 3, 16, 16 * 2));
            animPlayer.Add("jumpingSmall", new Animation(.1f, true, 16, 16, 1, 16 * 6, 16 * 2));
            animPlayer.Add("skidSmall", new Animation(1f, true, 16, 16, 1, 16 * 5, 16 * 2));
            animPlayer.Add("climbSmall", new Animation(.03f, true, 16, 16, 2, 16 * 8, 16 * 2));
            animPlayer.Add("killed", new Animation(1f, true, 16, 32, 1, 16 * 4, 16 * 2));

            animPlayer.Add("idleBig", new Animation(1f, true, 16, 32, 1, 0, 0));
            animPlayer.Add("runningBig", new Animation(.04f, true, 16, 32, 3, 16, 0));
            animPlayer.Add("jumpingBig", new Animation(.1f, true, 16, 32, 1, 16 * 6, 0));
            animPlayer.Add("skidBig", new Animation(1f, true, 16, 32, 1, 16 * 5, 0));
            animPlayer.Add("climbBig", new Animation(.03f, true, 16, 32, 2, 16 * 8, 0));

            animPlayer.Add("crouching", new Animation(1f, true, 16, 32, 1, 16 * 7, 0));

            animPlayer.Add("firing", new Animation(0.04f, false, 16, 32, 1, 16 * 4, 0));

            animPlayer.Add("shrinking", new Animation(.03f, true, 16, 32, 2, 48, 48));

            {
                Animation growing = new Animation();
                growing.AddFrame(0, 48, 16, 32, 0.1837f);
                growing.AddFrame(16, 48, 16, 32, 0.0668f);
                growing.AddFrame(0, 48, 16, 32, 0.0668f);
                growing.AddFrame(16, 48, 16, 32, 0.0668f);
                growing.AddFrame(0, 48, 16, 32, 0.0668f);
                growing.AddFrame(16, 48, 16, 32, 0.0668f);
                growing.AddFrame(32, 48, 16, 32, 0.0668f);
                growing.AddFrame(0, 48, 16, 32, 0.0668f);
                growing.AddFrame(16, 48, 16, 32, 0.0668f);
                growing.AddFrame(32, 48, 16, 32, 0.0668f);
                growing.AddFrame(0, 48, 16, 32, 0.0334f);
                growing.AddFrame(32, 48, 16, 32, 0.2004f);

                animPlayer.Add("growing", growing);
            }
        }

        public void Bounce()
        {
            bounce = true;
            currState = States.jump;
            velocity.Y = -bounceSpeed;
            animPlayer.PlayAnimation("jumping" + appended);
        }

        public override void Hit()
        {
            if (power > Power.none)
            {
                power = Power.none;
                IsInvencible = true;
                invencibleTimer = 0f;
                canTransform = true;
            }
            else
            {
                Destroy();
                if(deathEvent != null)
                {
                    deathEvent.Invoke();
                }
                //CanCollide = false;
                //Active = false;
                //IsDestroyed = true;
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
            SoundManager.ShrinkPipe.Play();

            prevState = currState;
            currState = States.shrinking;
            animPlayer.PlayAnimation("shrinking");
            AffectedByGravity = false;
            CanBeHit = false;
            CanCollide = false;
            secondCounter = 0f;
            prevVelocity = velocity;
            velocity = Vector2.Zero;
            IsTransforming = true;

            //texture = TextureManager.Player;
        }

        private void Burn()
        {
            SoundManager.Grow.Play();
            prevState = currState;
            currState = States.burning;
            origin = originBig;
            aabb = aabbBig;
            appended = "Big";
            AffectedByGravity = false;
            CanBeHit = false;
            CanCollide = false;
            secondCounter = 0f;
            prevVelocity = velocity;
            velocity = Vector2.Zero;
            IsTransforming = true;
            isBurning = true;
            //if (currState == States.fall)
            //    animPlayer.PlayAnimation("jumping" + appended);
        }

        private void Grow()
        {
            SoundManager.Grow.Play();
            origin = originBig;
            aabb = aabbBig;
            //texture = TextureManager.Player;

            prevState = currState;
            currState = States.growing;
            animPlayer.PlayAnimation("growing");
            AffectedByGravity = false;
            CanBeHit = false;
            CanCollide = false;
            secondCounter = 0f;
            prevVelocity = velocity;
            velocity = Vector2.Zero;
            IsTransforming = true;
        }

        public void GetStar()
        {
            starTimer = 0f;
            HasStar = true;
            MediaPlayer.Stop();
            MediaPlayer.Play(SoundManager.StarSong);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PrevPos = position;

            oldState = keyboard;
            keyboard = Keyboard.GetState();

            if (IsInvencible)
            {
                invencibleTimer += elapsed;
                if (invencibleTimer >= invencibleTotalTime)
                {
                    IsInvencible = false;
                }
            }
            // for debug purposes (switch to a transform)
            if (keyboard.IsKeyDown(Keys.G) && oldState.IsKeyUp(Keys.G))
            {
                //Burn();
                //power = Power.fire;
                GetStar();
            }

            // delay the transform switching as changing the bounding box takes
            // effect inmediately and it looks weird (draw position and real position is not the same)
            if (canTransform)
            {
                animPlayer.PlayAnimation(animPlayer.CurrentAnimation());

                if (power == Power.none)
                    Shrink();
                if (power == Power.big)
                    Grow();
                if (power == Power.fire)
                    Burn();

                canTransform = false;
            }

            switch (currState)
            {
                case States.stand:
                    UpdateStandState();
                    break;

                case States.run:
                    UpdateRunState();
                    break;

                case States.jump:
                    UpdateJumpState();
                    break;

                case States.fall:
                    UpdateFallState();
                    break;

                case States.firing:
                    UpdateFiringState();
                    break;

                case States.crouch:
                    UpdateCrouchState();
                    break;

                case States.growing:
                    UpdateGrowingState();
                    break;

                case States.shrinking:
                    UpdateShrinkingState();
                    break;

                case States.burning:
                    UpdateBurningState();
                    break;

                case States.downPipe:
                    UpdateDownPipeState();
                    break;

                case States.rightPipe:
                    UpdateRightPipeState();
                    break;

                case States.goal:
                    UpdateGoalState();
                    break;
            }

            if(HasStar)
            {
                starTimer += elapsed;
                int starTimerMod;

                if (starTimer < (starTotalTime * 0.8f))
                {
                    starTimerMod = 3;
                }
                else
                {
                    starTimerMod = 8;
                }

                if ((int)(starTimer * 60f) % starTimerMod == 0)
                {
                    colorStep++;
                    colorStep %= starColor.Length;
                }

                if (starTimer >= starTotalTime)
                {
                    starTimer = 0f;
                    HasStar = false;
                    IsInvencible = false;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(SoundManager.SurfaceStage);
                }
            }

            if (RightWallHit || LeftWallHit)
            {
                //speed = maxWalkSpeed;
                velocity.X = Math.Clamp(velocity.X, -maxWalkSpeed * .2f, maxWalkSpeed * .2f);
            }

            //vel.X = Math.Clamp(vel.X, -speed * elapsed, speed * elapsed);

            float updateSpeed = (currState == States.firing) ? 60 : Math.Max(50, Math.Abs(velocity.X)); // to change

            if (currState == States.growing || currState == States.shrinking)
                animPlayer.Update(elapsed);
            else
                animPlayer.Update(MapValue(maxRunSpeed, updateSpeed, elapsed));

            LateUpdate(gameTime);

            Area2D goal = HitArea(AreaType.goal);

            if (goal != null)
            {
                SoundManager.PoleDown.Play();
                currState = States.goal;
                animPlayer.PlayAnimation("climb" + appended);
                MediaPlayer.Stop();

                velocity = Vector2.Zero;
                velocity.Y = 100f;
                AffectedByGravity = false;

                goal.Active = false;
                goal = null;
            }

            if (CeilingHit)
            {
                Point tilePos = GetContactTile();

                World.Instance.HitTile(tilePos.X, tilePos.Y, power > Power.none);
            }

            secondCounter += elapsed;
        }

        private Area2D HitArea(AreaType areaType)
        {
            foreach (var a in World.Instance.GetTriggerAreas())
            {
                if (a.Active && a.Type == areaType)
                {
                    Rectangle pAABB = GetAABB();
                    Rectangle area = a.GetAABB();

                    if (pAABB.Intersects(area))
                        return a;
                }
            }
            return null;
        }

        private Area2D InsideArea(AreaType areaType)
        {
            foreach (var a in World.Instance.GetTriggerAreas())
            {
                if (a.Active && a.Type == areaType)
                {
                    Rectangle pAABB = GetAABB();
                    Rectangle area = a.GetAABB();

                    if (area.Contains(pAABB))
                        return a;
                }
            }
            return null;
        }

        private void GoDownPipe()
        {
            CanCollide = false;
            DrawBehind = true;
            currState = States.downPipe;
            velocity = Vector2.Zero;
        }

        private void GoRightPipe()
        {
            CanCollide = false;
            DrawBehind = true;
            AffectedByGravity = false;
            currState = States.rightPipe;
            velocity = Vector2.Zero;
            position.Y -= 0f;
        }

        public override void PreDraw(SpriteBatch spriteBatch)
        {
            if(target == null)
            {
                target = new RenderTarget2D(spriteBatch.GraphicsDevice, 16, 32);
            }

            spriteBatch.GraphicsDevice.SetRenderTarget(target);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(effect: null);

            
                animPlayer.Draw(
                    spriteBatch,
                    texture,
                    Vector2.Zero,
                    hFlip,
                    Vector2.Zero,
                    Color.White);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        //Color color;

        //if(HasStar)
        //{
        //    color = starColor[colorStep];
        //}
        //else
        //{
        //    color = Color.White;
        //}

        //if (invencibleTimer > 0f &&
        //    (int)(invencibleTimer * 60f) % 8 > 4)
        //{ }
        //else
        //{
        //    animPlayer.Draw(
        //        spriteBatch,
        //        texture,
        //        new Vector2((int)position.X, (int)position.Y),
        //        hFlip, 
        //        origin, 
        //        color);
        //}
            if (HasStar)
            {
                color = starColor[colorStep];
            }
            else if(isBurning)
            {
                color = starColor[colorStep];
            }
            else
            {
                color = Color.White;
            }

            if (invencibleTimer > 0f &&
                (int)(invencibleTimer * 60f) % 8 > 4)
            { }
            else
            {
                spriteBatch.Draw(target, new Vector2((int)position.X, (int)position.Y), null, color, 0, origin, 1, SpriteEffects.None, 1);

            }
            base.Draw(spriteBatch);
        }

        // pretty basic "lerp"
        private float Lerp(float start, float end, float percent)
        {
            float tmp = (start * (1f - percent)) + (end * percent);

            //if (Math.Abs(start) - Math.Abs(end) <= percent && Math.Abs(start) - Math.Abs(end) > 0)
            //    return end;

            return tmp;
        }

        private float MapValue(float maxValue, float minValue, float knowMaxValue)
        {
            return (minValue * knowMaxValue) / maxValue;
        }

        #region States

        private void UpdateStandState()
        {
            if ((int)velocity.X >= 1 || (int)velocity.X <= -1)
                animPlayer.PlayAnimation("running" + appended);
            else
                animPlayer.PlayAnimation("idle" + appended);


            velocity.X = Lerp(velocity.X, 0, 0.1f);

            if (!IsOnGround)
            {
                currState = States.fall;
                return;
            }

            if (keyboard.IsKeyDown(Keys.Right) != keyboard.IsKeyDown(Keys.Left))
            {
                currState = States.run;
                return;
            }
            else if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
            {
                animPlayer.PlayAnimation("jumping" + appended);

                currState = States.jump;

                velocity.Y = -jumpSpeed;
                IsOnGround = false;

                if (appended == "Small")
                    SoundManager.JumpSmall.Play();
                else
                    SoundManager.JumpBig.Play();

                return;
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                Area2D aDownPipe = InsideArea(AreaType.downPipe);

                if (aDownPipe != null)
                {
                    GoDownPipe();
                    SoundManager.ShrinkPipe.Play();
                    MediaPlayer.Stop();
                    aDownPipe.Active = false;
                    aDownPipe = null;

                    return;
                }

                if (power > Power.none)
                {
                    animPlayer.PlayAnimation("crouching");
                    currState = States.crouch;

                    return;
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

                    SoundManager.FireBall.Play();
                }

                return;
            }
        }

        private void UpdateRunState()
        {
            animPlayer.PlayAnimation("running" + appended);

            if (keyboard.IsKeyDown(Keys.A))
            {
                speed = maxRunSpeed;
            }
            else if (keyboard.IsKeyUp(Keys.A))
            {
                speed = maxWalkSpeed;
            }

            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
            {
                currState = States.stand;
                return;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                Area2D aRightPipe = InsideArea(AreaType.rightPipe);

                if (aRightPipe != null)
                {
                    GoRightPipe();
                    SoundManager.ShrinkPipe.Play();
                    MediaPlayer.Stop();
                    aRightPipe.Active = false;
                    aRightPipe = null;
                    return;
                }

                if (velocity.X < 0f)
                    animPlayer.PlayAnimation("skid" + appended);

                dir = 1f;
                velocity.X = Lerp(velocity.X, speed, 0.04f);

                hFlip = SpriteEffects.None;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                if (velocity.X > 0f)
                    animPlayer.PlayAnimation("skid" + appended);

                dir = -1f;
                velocity.X = Lerp(velocity.X, -speed, 0.04f);

                hFlip = SpriteEffects.FlipHorizontally;
            }

            if (keyboard.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
            {
                animPlayer.PlayAnimation("jumping" + appended);

                currState = States.jump;
                velocity.Y = -jumpSpeed;

                if (speed == maxRunSpeed)
                    gravity = maxGravity;

                IsOnGround = false;

                if (appended == "Small")
                    SoundManager.JumpSmall.Play();
                else
                    SoundManager.JumpBig.Play();

                return;
            }

            else if (!IsOnGround)
            {
                currState = States.fall;
                return;
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                Area2D aDownPipe = InsideArea(AreaType.downPipe);

                if (aDownPipe != null)
                {
                    GoDownPipe();
                    SoundManager.ShrinkPipe.Play();
                    MediaPlayer.Stop();
                    aDownPipe.Active = false;
                    aDownPipe = null;
                    return;
                }

                if (power > Power.none)
                {
                    currState = States.crouch;

                    return;
                }
            }

            if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
            {
                if (EntityManager.FireBallCount < 2)
                {
                    EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));

                    prevState = currState;
                    currState = States.firing;

                    SoundManager.FireBall.Play();
                }

                return;
            }
        }

        private void UpdateJumpState()
        {
            animPlayer.PlayAnimation("jumping" + appended);

            if (keyboard.IsKeyDown(Keys.Right))
            {
                velocity.X = Lerp(velocity.X, speed, 0.03f);
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                velocity.X = Lerp(velocity.X, -speed, 0.03f);
            }

            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
            {
                velocity.X = Lerp(velocity.X, 0, 0.1f);
            }

            if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
            {
                if (EntityManager.FireBallCount < 2)
                {
                    EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));
                    SoundManager.FireBall.Play();
                }
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

            if (velocity.Y > 0)
            {
                currState = States.fall;
                bounce = false;
                return;
            }
        }

        private void UpdateFallState()
        {
            gravity = normalGravity;

            animPlayer.Freeze();

            if (keyboard.IsKeyDown(Keys.Right))
            {
                velocity.X = Lerp(velocity.X, speed, 0.02f);
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                velocity.X = Lerp(velocity.X, -speed, 0.02f);
            }

            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
            {
                velocity.X = Lerp(velocity.X, 0, 0.1f);
            }

            if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && power == Power.fire)
            {
                if (EntityManager.FireBallCount < 2)
                {
                    EntityManager.Add(new FireBall(new Vector2(hFlip == 0 ? position.X + 6 : position.X - 6, position.Y - 23), hFlip == 0 ? 1f : -1f));
                    SoundManager.FireBall.Play();
                }
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
        }

        private void UpdateFiringState()
        {
            animPlayer.PlayAnimation("firing");

            if (animPlayer.AnimationEnded("firing"))
            {
                currState = prevState;
            }
        }

        private void UpdateCrouchState()
        {
            animPlayer.PlayAnimation("crouching");

            aabb = aabbSmall;

            velocity.X = Lerp(velocity.X, 0, 0.1f);

            if (!IsOnGround)
            {
                aabb = aabbBig;

                currState = States.fall;
                return;
            }

            if (keyboard.IsKeyUp(Keys.Down))
            {
                aabb = aabbBig;

                currState = States.stand;
                return;
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
                IsOnGround = false;
                return;
            }
        }

        private void UpdateGrowingState()
        {
            if (secondCounter >= transformationTotalTime)
            {
                appended = "Big";

                currState = prevState;
                CanBeHit = true;
                CanCollide = true;
                AffectedByGravity = true;
                velocity = prevVelocity;
                IsTransforming = false;
                IsInvencible = false;
                //Camera2D.Instance.RestoreZoom();
            }
        }

        private void UpdateShrinkingState()
        {
            if (secondCounter >= transformationTotalTime)
            {
                origin = originSmall;
                aabb = aabbSmall;
                appended = "Small";

                CanBeHit = true;
                CanCollide = true;
                AffectedByGravity = true;
                velocity = prevVelocity;
                IsTransforming = false;

                if ((int)velocity.Y != 0)
                {
                    currState = States.jump;
                    animPlayer.PlayAnimation("running" + appended);
                }
                else
                {
                    currState = States.run;
                    animPlayer.PlayAnimation("running" + appended);
                }
            }
        }


        private void UpdateBurningState()
        {
            if ((int)(secondCounter * 60f) % 8 == 0)
            {
                colorStep++;
                colorStep %= starColor.Length;
            }

            animPlayer.Freeze();

            if (secondCounter >= transformationTotalTime)
            {
                currState = prevState;
                CanBeHit = true;
                CanCollide = true;
                AffectedByGravity = true;
                velocity = prevVelocity;
                IsTransforming = false;
                IsInvencible = false;
                texture = TextureManager.PlayerFire;
                isBurning = false;
                //Camera2D.Instance.RestoreZoom();
            }
        }
            

        private void UpdateDownPipeState()
        {
            velocity.Y = pipeSpeed;

            if (position.Y >= 200)
            {
                position = new Vector2(50f, 0f);
                velocity = Vector2.Zero;
                currState = States.jump;
                hFlip = SpriteEffects.None;
                CanCollide = true;
                DrawBehind = false;
                AffectedByGravity = true;
                EntityManager.StartOver();
                World.Instance.LoadLevel("Content\\Levels\\stage1bonus.tmx");
                //MediaPlayer.Play(SoundManager.UnderGroundStage);
            }
        }

        private void UpdateRightPipeState()
        {
            animPlayer.PlayAnimation("running" + appended);
            velocity.X = maxWalkSpeed * 0.5f;

            if (position.X >= 300)
            {
                EntityManager.StartOver();
                World.Instance.LoadLevel("Content\\Levels\\stage1.tmx");
                position = new Vector2(2480, 176);
                velocity = Vector2.Zero;
                currState = States.stand;
                MediaPlayer.Play(SoundManager.SurfaceStage);
                CanCollide = true;
                DrawBehind = false;
                AffectedByGravity = true;
                hFlip = SpriteEffects.None;
            }
        }

        private void UpdateGoalState()
        {
            if (FloorHit)
            {
                AffectedByGravity = true;
                animPlayer.PlayAnimation("running" + appended);
                velocity.X = maxWalkSpeed;
            }

            foreach (var a in World.Instance.GetTriggerAreas())
            {
                Rectangle pAABB = GetAABB();
                Rectangle aAABB = a.GetAABB();

                if (aAABB.Contains(pAABB) && a.Type == AreaType.endStage)
                {
                    velocity = Vector2.Zero;
                    DrawBehind = true;
                    animPlayer.Freeze();
                }
            }
        }

        #endregion
    }
}

