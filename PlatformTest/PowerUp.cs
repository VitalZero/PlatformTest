using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public enum PowerupType { oneup, coin, mushroom, flower, star }

    public class PowerUp : Entity
    {
        protected PowerupType type;
        protected Rectangle spriteArea;
        protected float riseTime = 1f;
        protected float riseStart = 0;
        protected float startY;
        protected int yOffset;

        public PowerUp(PowerupType type, Vector2 pos)
        {
            this.type = type;
            spriteArea = new Rectangle();
            this.position = pos;
            pos.X = (int)pos.X;
            pos.Y = (int)pos.Y;
            CanCollide = false;
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
