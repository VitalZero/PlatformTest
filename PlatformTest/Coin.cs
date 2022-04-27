using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Coin : Item
    {
        public Coin(Vector2 pos)
            : base(ItemType.coin, pos)
        {
            CanCollide = false;
            CanBeHit = true;
        }

        public override void Init()
        {
            base.Init();
            spriteArea = new Rectangle(16, 0, 16, 16);
            aabb = new Rectangle(3, 1, 10, 14);
        }

        public override void Collected()
        {
            SoundManager.Coin.Play();
            Destroy();
        }
    }
}
