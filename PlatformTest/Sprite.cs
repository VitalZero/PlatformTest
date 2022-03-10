using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public abstract class Sprite : GameObject
    {
        protected Vector2 vel;
        protected bool destroyed;
        protected float timeCounter;
        protected float lifeTime;
        public bool IsDestroyed { get { return destroyed; } }

        public abstract void Init();
    }
}
