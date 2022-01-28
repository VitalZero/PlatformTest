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
        private Vector2 pos;
        private Vector2 vel;
        private Vector2 dir;
        private const float speed = 100f;
        KeyboardState keyState;

        public Player()
        {
            pos = new Vector2(50f, 50f);
            vel = Vector2.Zero;
            dir = Vector2.Zero;
        }

        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("mariobasic");
        }

        public void Input(GameTime gameTime)
        {
            keyState = Keyboard.GetState();
            dir = Vector2.Zero;
            
            if(keyState.IsKeyDown(Keys.Right))
                dir.X = 1;
            if (keyState.IsKeyDown(Keys.Left))
                dir.X = -1;
            if (keyState.IsKeyDown(Keys.Up))
                dir.Y = -1;
            if (keyState.IsKeyDown(Keys.Down))
                dir.Y = 1;
        }

        public void Update(GameTime gameTime)
        {
            vel = dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            pos += vel;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                pos,
                new Rectangle(0, 0, 16, 32),
                Color.White
                );
        }
    }
}
