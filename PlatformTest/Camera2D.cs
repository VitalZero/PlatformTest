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
        private bool shake;
        private float shakeRadius;
        private float shakeAmount;
        private Vector2 shakeOffset;
        Random rand;
        float startShakeAngle;

        public Camera2D(Rectangle bounds)
        {
            this.bounds = bounds;
            instance = this;
            shake = false;
            shakeOffset = Vector2.Zero;
            shakeRadius = 0f;
            rand = new Random();
            startShakeAngle = MathHelper.PiOver2;
            shakeRadius = 2f;
            shakeAmount = shakeRadius / 60;
        }

        public void Shake()
        {
            shake = true;
        }

        public Vector2 WorldToScreen(Vector2 pos)
        {
            return new Vector2(pos.X + Transform.Translation.X, pos.Y + Transform.Translation.Y);
        }

        public void Follow(Entity entity)
        {
            int pX = (int)entity.Position.X;

            if (pX < (bounds.Width / 2))
                pX = bounds.Width / 2;
            else if (pX > (World.Instance.mapWidth * 16) - (bounds.Width / 2))
                pX = (World.Instance.mapWidth * 16) - (bounds.Width / 2);

            Matrix position = Matrix.CreateTranslation(
                -pX,
                0,
                0);

            Matrix offset = Matrix.CreateTranslation(
                bounds.Width / 2,
                0,
                0);

            if (shake)
            {
                shakeOffset = new Vector2((float)Math.Sin(startShakeAngle) * shakeRadius, (float)Math.Cos(startShakeAngle) * shakeRadius);
                shakeRadius -= shakeAmount;
                startShakeAngle += (100 + rand.Next(60));

                if (shakeRadius < 0)
                {
                    shakeRadius = 2f;
                    shake = false;
                    shakeOffset = Vector2.Zero;
                    startShakeAngle = MathHelper.PiOver2;
                }
            }

            Matrix cameraShake = Matrix.CreateTranslation(new Vector3(shakeOffset, 0));

            Transform = position * offset * cameraShake;
        }
    }
}
