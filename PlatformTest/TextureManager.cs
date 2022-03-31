using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public static class TextureManager
    {
        public static Texture2D Player { get; private set; }
        public static Texture2D Goomba { get; private set; }
        public static Texture2D Level { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Turtle { get; private set; }
        public static Texture2D Items { get; private set; }
        public static Texture2D MiscSprites { get; private set; }
        public static Effect ColorSwap { get; private set; }
        public static Texture2D SourcePal { get; private set; }
        public static Texture2D Pal1 { get; private set; }
        public static Texture2D Pal2 { get; private set; }

        public static SpriteFont Arial { get; private set; }

        public static void Load(ContentManager content)
        {
            Player = content.Load<Texture2D>("Sprites/mariobasic");
            Goomba = content.Load<Texture2D>("Sprites/goomba");
            Level = content.Load<Texture2D>("Tiles/mariotilesimple");
            Pixel = content.Load<Texture2D>("Sprites/pixel");
            Turtle = content.Load<Texture2D>("Sprites/koopatrooper");
            Items = content.Load<Texture2D>("Sprites/marioitems");
            MiscSprites = content.Load<Texture2D>("Sprites/marioeffects");
            ColorSwap = content.Load<Effect>("ColorSwap");
            SourcePal = content.Load<Texture2D>("Palettes/sourcepalete1");
            Pal1 = content.Load<Texture2D>("Palettes/palete1");
            Pal2 = content.Load<Texture2D>("Palettes/palete2");

            Arial = content.Load<SpriteFont>("Arial");
        }
    }
}
