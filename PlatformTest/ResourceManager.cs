using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public static class ResourceManager
    {
        public static Texture2D Player { get; private set; }
        public static Texture2D Goomba { get; private set; }
        public static Texture2D Level { get; private set; }
        public static Texture2D Pixel { get; private set; }

        public static SpriteFont Arial { get; private set; }

        public static void Load(ContentManager content)
        {
            Player = content.Load<Texture2D>("mariobasic");
            Goomba = content.Load<Texture2D>("goomba");
            Level = content.Load<Texture2D>("mariotilessimple");
            Pixel = content.Load<Texture2D>("pixel");

            Arial = content.Load<SpriteFont>("Arial");
        }
    }
}
