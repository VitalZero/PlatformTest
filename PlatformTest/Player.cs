//#define DEBUG_DRAW

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

#if DEBUG_DRAW
        Texture2D debugTexture;
#endif
        private Vector2 origin;
        private float dir;
        private KeyboardState keyboard;
        private bool isJumping;
        private SpriteEffects flip;
        private Animation standing;
        private Animation run;
        private Animation jump;
        private Animation fall;
        private AnimationPlayer animPlayer;
        States currState;
        float lerpDuration;
        float stopLerp;
        float jumpLerp;
        float lerpTime;
        public bool Pause { get; set; }

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
            aabb = new Rectangle(2, 4, 12, 27);
            origin = new Vector2(size.X / 2, size.Y);
            animPlayer = new AnimationPlayer();
            isOnGround = false;
            Pause = false;

            instance = this;

            lerpDuration = 1f;
            stopLerp = .3f;
            jumpLerp = 2f;
            lerpTime = 0f;

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

            standing = new Animation(texture, 1f, true, 16, 1, 0, 0);
            run = new Animation(texture, .04f, true, 16, 4, 16, 0);
            jump = new Animation(texture, .1f, true, 16, 1, 16 * 6, 0);
            fall = new Animation(texture, 1f, true, 16, 1, 16 * 5, 0);
            animPlayer.PlayAnimation(standing);
            font = ResourceManager.Arial;
        }

        public void Bounce()
        {
            vel.Y = (jumpSpeed / 2f) * elapsed;
        }

        public override void Hit()
        {
            CanCollide = false;
            Active = false;
        }

        public override void Update(GameTime gameTime)
        {
            lerpTime += elapsed;

            KeyboardState oldState = keyboard;
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A))
                Pause = !Pause;

            if (Pause)
                return;

            ApplyGravity();

            switch (currState)
            {
                case States.stand:
                    {
                        animPlayer.PlayAnimation(standing);

                        vel.X = 0;

                        if (!isOnGround)
                        {
                            currState = States.fall;
                            lerpTime = 0f;
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Right) != keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.run;
                            lerpTime = 0;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        {
                            currState = States.jump;
                            lerpTime = 0;
                            vel.Y = jumpSpeed * elapsed;
                            isOnGround = false;
                            break;
                        }
                    }
                    break;

                case States.run:
                    {
                        animPlayer.PlayAnimation(run);

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.stand;
                            lerpTime = 0;
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

                        if (keyboard.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        {
                            currState = States.jump;
                            lerpTime = 0;
                            vel.Y = jumpSpeed * elapsed;
                            isOnGround = false;
                            break;
                        }
                        else if (!isOnGround)
                        {
                            currState = States.fall;
                            lerpTime = 0;
                            break;
                        }
                    }
                    break;

                case States.jump:
                    {
                        animPlayer.PlayAnimation(jump);

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

                        if(isOnGround)
                        {
                            if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                            {
                                currState = States.stand;
                                lerpTime = 0;
                                vel = Vector2.Zero;
                                break;
                            }
                            else
                            {
                                currState = States.run;
                                lerpTime = 0;
                                vel.Y = 0f;
                                break;
                            }
                        }

                        if(vel.Y > 0)
                        {
                            currState = States.fall;
                            lerpTime = 0f;
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
                            lerpTime = 0f;
                            vel = Vector2.Zero;
                        }
                        else
                        {
                            currState = States.run;
                            lerpTime = 0f;
                            vel.Y = 0f;
                        }
                    }

                    break;
            }

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);

            if (CeilingHit)
            {
                //Pause = true;
                Point tilePos = GetContactTile();
                Tile t = Map.Instance.GetTile(tilePos.X, tilePos.Y);

                if (t.collision == TileCollision.breakable)
                    Map.Instance.RemoveTile(tilePos.X, tilePos.Y);
                else if (t.collision == TileCollision.item)
                    Map.Instance.usedTileItem(tilePos.X, tilePos.Y);
            }

            isJumping = false;

            dir = 0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch, 
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset), 
                flip);

            spriteBatch.DrawString(font, playerStates[(int)currState], 
                new Vector2((int)pos.X - 10 - (int)Camera.Instance.XOffset, (int)pos.Y - 20 - (int)Camera.Instance.YOffset), Color.Crimson);

#if DEBUG_DRAW
            Rectangle aabbDebug = GetAABB();
            //aabbDebug.X += (int)-camera.XOffset;
            //aabbDebug.Y += (int)-camera.YOffset;
            //spriteBatch.Draw(debugTexture, aabbDebug, new Color(Color.Red, 0.2f));

            spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)Camera.Instance.XOffset, ((aabbDebug.Top / 16) * 16) - (int)Camera.Instance.YOffset, 16, 16),
                new Color(Color.White, 0.2f));
            spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)Camera.Instance.XOffset, ((aabbDebug.Bottom / 16) * 16) - (int)Camera.Instance.YOffset, 16, 16),
                new Color(Color.Green, 0.2f));

            spriteBatch.Draw(debugTexture, new Rectangle(
                (int)(pos.X + origin.X - camera.XOffset) - 1, 
                (int)(pos.Y + origin.Y - camera.YOffset) - 1,
                2, 2),
                new Color(Color.White, 0.8f));
#endif
        }
    }
}
