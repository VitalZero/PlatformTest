using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public abstract class Sprite : GameObject
    {
        protected Vector2 velocity;
        protected float timeCounter;
        protected float lifeTime;

        public abstract void Init();
    }
}
