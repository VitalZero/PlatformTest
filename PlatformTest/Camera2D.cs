using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Camera2D
    {
        private static Camera2D instance = null;
        public static Camera2D Instance { get { return instance; } }
        public Matrix Transform { get; private set; }
        public Matrix CameraShake { get; private set; }
        public Matrix Zoom { get; private set; }
        public Vector2 Position { get; private set; }
        private Rectangle bounds;
        private Rectangle screenSize;
        private bool shake;
        private const float initialShakeRadius = 2f;
        private float shakeRadius;
        private float shakeAmount;
        private Vector2 shakeOffset;
        float startShakeAngle;
        private float zoom;
        private bool zooming;
        private readonly float defaultZoom = 1f;
        Random rand;

        public Camera2D(Rectangle bounds, Rectangle screenSize)
        {
            this.bounds = bounds;
            this.screenSize = screenSize;
            instance = this;
            shake = false;
            shakeOffset = Vector2.Zero;
            rand = new Random();
            startShakeAngle = MathHelper.PiOver2;
            shakeRadius = initialShakeRadius;
            shakeAmount = shakeRadius / 25;
            zoom = defaultZoom;
            zooming = false;
        }

        public void Shake()
        {
            shake = true;
        }

        public Vector2 WorldToScreen(Vector2 pos)
        {
            return new Vector2(pos.X + Transform.Translation.X, pos.Y + Transform.Translation.Y);
        }

        public void SetZoom(float zoomValue)
        {
            zoom = (float)Math.Clamp(zoomValue, 0.1f, 10f);
            zooming = true;
        }

        public void RestoreZoom()
        {
            zoom = defaultZoom;
            zooming = false;
        }

        public void Follow(Entity entity, float dt)
        {
            if (shake)
            {
                shakeOffset = new Vector2((float)Math.Sin(startShakeAngle) * shakeRadius, (float)Math.Cos(startShakeAngle) * shakeRadius);
                shakeRadius -= shakeAmount;
                startShakeAngle += (100 + rand.Next(60));

                if (shakeRadius < 0)
                {
                    shakeRadius = initialShakeRadius;
                    shake = false;
                    shakeOffset = Vector2.Zero;
                    startShakeAngle = MathHelper.PiOver2;
                }
            }

            CameraShake = Matrix.CreateTranslation(new Vector3((int)shakeOffset.X, (int)shakeOffset.Y, 0));

            int pX = (int)entity.Position.X;
            int pY = (int)entity.Position.Y;

            pX = (int)Math.Clamp(pX, bounds.Width / 2, (World.Instance.mapWidth * 16) - (bounds.Width / 2));
    
            if(!zooming)
                pY = (int)Math.Clamp(pY, bounds.Height / 2, (World.Instance.mapHeight * 16) - (bounds.Height / 2));

            Matrix matrixPos = Matrix.CreateTranslation(
                -pX,
                -pY,
                0);

            Matrix matrixOffset = Matrix.CreateTranslation(
                bounds.Width / 2,
                bounds.Height / 2,
                0);

            Zoom = Matrix.CreateScale(zoom, zoom, 1f);

            Position = new Vector2((screenSize.Width / 2) - matrixPos.Translation.X, (screenSize.Height / 2) - matrixPos.Translation.Y);

            Transform = matrixPos * Zoom * matrixOffset;
        }
    }
}
