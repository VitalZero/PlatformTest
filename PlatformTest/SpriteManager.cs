using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformTest
{
    public static class SpriteManager
    {
        static private List<Sprite> sprites = new List<Sprite>();
        static private List<Sprite> addedSprites = new List<Sprite>();

        static public int Count { get { return sprites.Count; } }

        static private bool isUpdating;

        public static void Add(Sprite sprite)
        {
            //if (!isUpdating)
            //{
            //    AddSprite(sprite);
            //}
            //else
            //{
            //    addedSprites.Add(sprite);
            //}
            addedSprites.Add(sprite);
        }

        public static void RemoveDestroyedSprites()
        {
            sprites = sprites.Where(e => !e.IsDestroyed).ToList();
        }

        private static void AddSprite(Sprite sprite)
        {
            sprite.Init();
            sprites.Add(sprite);
        }

        public static void Update(GameTime gameTime)
        {
            isUpdating = true;

            foreach(var s in sprites)
            {
                s.Update(gameTime);
            }

            isUpdating = false;

            foreach(var s in addedSprites)
            {
                AddSprite(s);
            }

            RemoveDestroyedSprites();

            addedSprites.Clear();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach(var s in sprites)
            {
                s.Draw(spriteBatch);
            }
        }
    }
}
