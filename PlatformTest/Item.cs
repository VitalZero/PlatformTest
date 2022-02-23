﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public enum ItemType { oneup, coin, mushroom, flower, star }

    public class Item : Entity
    {
        ItemType type;
        Rectangle spriteArea;
        float riseTime = 1f;
        float riseStart = 0;

        public Item(ItemType type, Vector2 pos)
        {
            this.type = type;
            spriteArea = new Rectangle();
            this.pos = pos;
            CanCollide = false;
            CanKill = false;
            dir = 1f;
        }

        public override void Init()
        {
            texture = ResourceManager.Items;
            spriteArea = new Rectangle((int)type * 16, 0, 16, 16);
            aabb = new Rectangle(0, 0, 16, 16);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, 
                new Vector2((int)pos.X -(int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                spriteArea, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            if(type != ItemType.coin)
            {
                if(riseStart <= riseTime)
                {
                    riseStart += elapsed;
                    pos.Y -= 20f * elapsed;
                }
                else
                {
                    vel.X = speed * elapsed * dir;
                    CanCollide = true;

                    if (RightWallHit)
                    {
                        dir = -1f;
                    }
                    else if (LeftWallHit)
                    {
                        dir = 1f;
                    }
                }
            }

            LateUpdate(gameTime);
        }
    }
}
