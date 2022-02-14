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
                return instance;
            }
        }
        private enum States { stand, run, jump, fall }

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
        public bool Pause { get; set; }

        //for debug purposes
        List<string> playerStates = new List<string>();
        SpriteFont font;
        //

        public Player(Map map, Camera camera)
            : base(map, camera)
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

            currState = States.fall;

            //for debug
            playerStates.Add("STAND");
            playerStates.Add("RUN");
            playerStates.Add("JUMP");
            playerStates.Add("FALL");
            //
        }

        public void Load(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

#if DEBUG_DRAW
            debugTexture = content.Load<Texture2D>("pixel");
#endif

            texture = content.Load<Texture2D>("mariobasic");

            standing = new Animation(texture, 1f, true, 16, 1, 0, 0);
            run = new Animation(texture, .1f, true, 16, 4, 16, 0);
            jump = new Animation(texture, .1f, true, 16, 1, 16 * 6, 0);
            fall = new Animation(texture, 1f, true, 16, 1, 16 * 5, 0);
            animPlayer.PlayAnimation(standing);
            font = content.Load<SpriteFont>("Arial");
        }

        public override void Update(GameTime gameTime)
        {
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
                            break;
                        }

                        if (keyboard.IsKeyDown(Keys.Right) != keyboard.IsKeyDown(Keys.Left))
                        {
                            currState = States.run;
                            break;
                        }
                        else if (keyboard.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        {
                            currState = States.jump;
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
                            vel.X = 0;
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
                            vel.Y = jumpSpeed * elapsed;
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
                        animPlayer.PlayAnimation(jump);

                        if (keyboard.IsKeyDown(Keys.Right) == keyboard.IsKeyDown(Keys.Left))
                        {
                            vel.X = 0f;
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
                        vel.X = 0f;
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

            animPlayer.Update(gameTime);

            LateUpdate(gameTime);

            if (CeilingHit)
            {
                //Pause = true;
                Point tilePos = GetContactTile();
                Tile t = map.GetTile(tilePos.X, tilePos.Y);

                if (t.collision == TileCollision.breakable)
                    map.RemoveTile(tilePos.X, tilePos.Y);
                else if (t.collision == TileCollision.item)
                    map.usedTileItem(tilePos.X, tilePos.Y);
            }

            isJumping = false;

            dir = 0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch, 
                new Vector2((int)pos.X - (int)camera.XOffset, (int)pos.Y - (int)camera.YOffset), 
                flip);

            spriteBatch.DrawString(font, playerStates[(int)currState], 
                new Vector2((int)pos.X - 10 - (int)camera.XOffset, (int)pos.Y - 20 - (int)camera.YOffset), Color.Crimson);

#if DEBUG_DRAW
            Rectangle aabbDebug = GetAABB();
            //aabbDebug.X += (int)-camera.XOffset;
            //aabbDebug.Y += (int)-camera.YOffset;
            //spriteBatch.Draw(debugTexture, aabbDebug, new Color(Color.Red, 0.2f));

            spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)camera.XOffset, ((aabbDebug.Top / 16) * 16) - (int)camera.YOffset, 16, 16),
                new Color(Color.White, 0.2f));
            spriteBatch.Draw(debugTexture, new Rectangle(((aabbDebug.Right / 16) * 16) - (int)camera.XOffset, ((aabbDebug.Bottom / 16) * 16) - (int)camera.YOffset, 16, 16),
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
