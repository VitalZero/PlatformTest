using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Camera2D
    {
        protected float zoom;
        protected float rotation;
        public Matrix transform;
        public Vector2 pos;
        protected Rectangle bounds;

        public Camera2D(Viewport viewport)
        {
            zoom = 1f;
            rotation = 0f;
            pos = Vector2.Zero;
            bounds = viewport.Bounds;
        }

        public Matrix GetTransformation()
        {
            transform = Matrix.CreateTranslation(new Vector3(-pos.X, -120, 0)) * 
                Matrix.CreateScale(4f) *
                Matrix.CreateTranslation(new Vector3(bounds.Width * 0.5f, bounds.Height * 0.5f, 0));

            return transform;
        }
    }
}
