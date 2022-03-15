using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public enum ItemType { oneup, coin, mushroom, flower, star }

    public class PowerUp : Entity
    {
        protected ItemType type;
        protected Rectangle spriteArea;
        protected float riseTime = 1f;
        protected float riseStart = 0;
        protected float startY;
        protected int yOffset;

        public PowerUp(ItemType type, Vector2 pos)
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
            IsDestroyed = true;
            Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, 
                new Vector2((int)position.X -(int)Camera.Instance.XOffset, (int)position.Y - (int)Camera.Instance.YOffset),
                spriteArea, Color.White);
        }
    }
}
