using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Camera2D
    {
        public Matrix Transform { get; private set; }
        private Rectangle bounds;
        private static Camera2D instance = null;
        public static Camera2D Instance { get { return instance; } }

        public Camera2D(Rectangle bounds)
        {
            this.bounds = bounds;
            instance = this;
        }

        public void Follow(Entity entity)
        {
            int pX = (int)entity.Position.X;

            if (pX < (bounds.Width / 2))
                pX = bounds.Width / 2;

            Matrix position = Matrix.CreateTranslation(
                -pX,
                0,
                0);

            Matrix offset = Matrix.CreateTranslation(
                bounds.Width / 2,
                0,
                0);

            Transform = position * offset;
        }
    }
}
