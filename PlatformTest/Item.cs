using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public enum ItemType { oneup, coinBox, mushroom, flower, star, coin }

    public class Item : Entity
    {
        protected ItemType type;
        protected Rectangle spriteArea;
        protected float riseTime = 1f;
        protected float riseStart = 0;
        protected float startY;
        protected int yOffset;

        public Item(ItemType type, Vector2 pos)
        {
            this.type = type;
            spriteArea = new Rectangle();
            this.position = pos;
            position.X = (int)position.X;
            position.Y = (int)position.Y;
            CanCollide = false;
            startY = position.Y;
        }

        public ItemType GetItemType()
        {
            return type;
        }

        public override void Init()
        {
            texture = TextureManager.Items;
            spriteArea = new Rectangle((int)type * 16, 0, 16, 16);
            aabb = new Rectangle(3, 4, 10, 11);
        }

        public virtual void Collected() 
        {
            Destroy();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, 
                new Vector2((int)position.X, (int)position.Y),
                spriteArea, Color.White);
        }
    }
}
