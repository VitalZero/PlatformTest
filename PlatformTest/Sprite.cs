using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public abstract class Sprite : GameObject
    {
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected float timeCounter;
        protected float lifeTime;

        public abstract void Init();

        public void SetVelocity(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public void SetAcceleration(Vector2 acceleration)
        {
            this.acceleration = acceleration;
        }
    }
}
