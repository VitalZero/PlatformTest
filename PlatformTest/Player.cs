using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Player
    {
        private Texture2D texture;
        public Vector2 Pos { get { return pos; } }
        private Vector2 pos;
        private Vector2 vel;
        private Vector2 origin;
        private Vector2 size;
        private float dir;
        private const float speed = 100f;
        private const float gravity = 20f;
        private const float jumpSpeed = -400f;
        KeyboardState keyState;
        ContentManager content;
        private bool isOnGround = false;
        private bool isJumping;
        private Rectangle aabb;
        Camera camera;
        SpriteEffects flip;
        Animation standing;
        Animation run;
        Animation jump;
        AnimationPlayer animPlayer;

        public Player(Camera camera)
        {
            pos = new Vector2(50f, 50f);
            size = new Vector2(16, 31);
            vel = Vector2.Zero;
            dir = 0f;
            aabb = new Rectangle(2, 4, 12, 27);
            origin = new Vector2(size.X / 2, size.Y);
            this.camera = camera;
            animPlayer = new AnimationPlayer();
        }

        public void Load(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            texture = content.Load<Texture2D>("mariobasic");

            standing = new Animation(texture, 1f, true, 16, 1, 0, 0);
            run = new Animation(texture, .1f, true, 16, 4, 16, 0);
            jump = new Animation(texture, .1f, true, 16, 1, 16 * 6, 0);
            animPlayer.PlayAnimation(standing);
        }

        public void Input(GameTime gameTime)
        {
            KeyboardState oldState = keyState;
            keyState = Keyboard.GetState();

            //isJumping = keyState.IsKeyDown(Keys.Space);
            if (keyState.IsKeyDown(Keys.Space))
                isJumping = true;

            if (keyState.IsKeyDown(Keys.Right))
                dir = 1f;
            if (keyState.IsKeyDown(Keys.Left))
                dir = -1f;
        }

        public void Update(GameTime gameTime, Map map)
        {
            Physics(gameTime, map);

            if (vel.X > 0)
            {
                flip = SpriteEffects.None;
                if (isOnGround)
                    animPlayer.PlayAnimation(run);
            }
            else if (vel.X < 0)
            {
                flip = SpriteEffects.FlipHorizontally;
                if (isOnGround)
                    animPlayer.PlayAnimation(run);
            }
            else
            {
                if(isOnGround)
                    animPlayer.PlayAnimation(standing);
            }

            animPlayer.Update(gameTime);

            isJumping = false;

            dir = 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(
            //    texture,
            //    new Vector2(pos.X - camera.XOffset, pos.Y - camera.YOffset),
            //    new Rectangle(0, 0, 16, 32),
            //    Color.White,
            //    0,
            //    Vector2.Zero,
            //    1,
            //    flip,
            //    0
            //    );
            animPlayer.Draw(spriteBatch, new Vector2(pos.X - camera.XOffset + origin.X, pos.Y - camera.YOffset + origin.Y), flip);
        }

        private Rectangle GetAABB()
        {
            return new Rectangle((int)pos.X + aabb.X + (int)origin.X, (int)pos.Y + aabb.Y + (int)origin.Y, aabb.Width, aabb.Height);
        }

        private void Physics(GameTime gameTime, Map map)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            vel.X = dir * speed * elapsed;
            vel.Y = MathHelper.Clamp(vel.Y + gravity * elapsed, -10f, 10f);

            if (isJumping && isOnGround)
            {
                //if (isOnGround)
                {
                    animPlayer.PlayAnimation(jump);
                    vel.Y = jumpSpeed * elapsed;
                }
            }

            pos.X += vel.X;
            pos.X = (float)Math.Round(pos.X);

            HandleCollisionsX(map);

            pos.Y += vel.Y;
            pos.Y = (float)Math.Round(pos.Y);

            isOnGround = false;

            HandleCollisionsY(map);
        }

        private void HandleCollisionsX(Map map)
        {
            Rectangle bounds = GetAABB();

            int top = (int)(bounds.Top / 16);
            int bottom = (int)((bounds.Bottom) / 16);
            int left = (int)(bounds.Left / 16);
            int right = (int)((bounds.Right) / 16);

            if (top < 0) top = 0;
            if (left < 0) left = 0;
            if (right >= map.mapWidth) right = map.mapWidth;
            if (bottom >= map.mapHeight) bottom = map.mapHeight;

            List<Tile> tilesToCheck = new List<Tile>();

            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    Tile tile = map.GetTile(x, y);

                    if(tile.collision != TileCollision.none)
                        tilesToCheck.Add(tile);
                }
            }

            foreach (var t in tilesToCheck)
            {
                Rectangle tileBounds = new Rectangle(t.X * 16, t.Y * 16, 16, 16);

                if (bounds.Intersects(tileBounds))
                {
                    float leftDist = Math.Abs(bounds.Right - tileBounds.Left);
                    float rightDist = Math.Abs(bounds.Left - tileBounds.Right);

                    if (leftDist < rightDist)
                    {
                        pos.X += -leftDist;
                    }
                    else
                    {
                        pos.X += rightDist;
                    }

                    break;
                }

            }
        }

        private void HandleCollisionsY(Map map)
        {
            Rectangle bounds = GetAABB();

            int top = (int)(bounds.Top / 16);
            int bottom = (int)((bounds.Bottom) / 16);
            int left = (int)(bounds.Left / 16);
            int right = (int)((bounds.Right) / 16);

            if (top < 0) top = 0;
            if (left < 0) left = 0;
            if (right >= map.mapWidth) right = map.mapWidth;
            if (bottom >= map.mapHeight) bottom = map.mapHeight;

            List<Tile> tilesToCheck = new List<Tile>();

            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    Tile tile = map.GetTile(x, y);

                    if (tile.collision != TileCollision.none)
                        tilesToCheck.Add(tile);
                }
            }

            foreach (var t in tilesToCheck)
            {
                Rectangle tileBounds = new Rectangle(t.X * 16, t.Y * 16, 16, 16);

                if (bounds.Intersects(tileBounds))
                {
                    float topDist = Math.Abs(bounds.Bottom - tileBounds.Top);
                    float bottomDist = Math.Abs(bounds.Top - tileBounds.Bottom);

                    if (topDist < bottomDist)
                    {
                        pos.Y += -topDist;
                        isOnGround = true;
                        vel.Y = 0;
                    }
                    else
                    {
                        if (t.collision == TileCollision.breakable)
                            map.RemoveTile((int)t.X, (int)t.Y);
                        else if (t.collision == TileCollision.item)
                            map.usedTileItem((int)t.X, (int)t.Y);

                        pos.Y += bottomDist;
                        vel.Y = 0;
                    }

                    break;
                }

            }
        }

        private Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) > minDistanceX || Math.Abs(distanceY) > minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}
