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
        public Matrix CameraShake { get; private set; }
        public Matrix Zoom { get; private set; }
        public Vector2 Position { get; private set; }
        private Rectangle bounds;
        private Rectangle screenSize;
        private static Camera2D instance = null;
        public static Camera2D Instance { get { return instance; } }
        private bool shake;
        private bool dramaticZoom;
        private const float initialShakeRadius = 2f;
        private float shakeRadius;
        private float shakeAmount;
        private Vector2 shakeOffset;
        private readonly float defaultZoom = 1f;
        private float zoom;
        private Vector2 zoomTarget;
        Random rand;
        float startShakeAngle;

        public Camera2D(Rectangle bounds, Rectangle screenSize)
        {
            this.bounds = bounds;
            this.screenSize = screenSize;
            instance = this;
            shake = false;
            dramaticZoom = false;
            shakeOffset = Vector2.Zero;
            rand = new Random();
            startShakeAngle = MathHelper.PiOver2;
            shakeRadius = initialShakeRadius;
            shakeAmount = shakeRadius / 25;
            zoom = defaultZoom;
        }

        public void Shake()
        {
            shake = true;
        }

        public void DramaticZoom(Vector2 target)
        {
            dramaticZoom = !dramaticZoom;
            zoomTarget = target;
            zoom = 2f;
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

            Matrix matrixPos = Matrix.CreateTranslation(
                -pX,
                0,
                0);

            Matrix matrixOffset = Matrix.CreateTranslation(
                bounds.Width / 2,
                0,
                0);

            Position = new Vector2((screenSize.Width / 2) - matrixPos.Translation.X, screenSize.Height / 2);

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

            if (dramaticZoom)
            {
                Zoom = Matrix.CreateTranslation(new Vector3(zoomTarget.X - screenSize.Width / 2, zoomTarget.Y - screenSize.Height / 2, 0)) * Matrix.CreateScale(zoom);
            }
            else
            {
                Zoom = Matrix.Identity;
            }

            CameraShake = Matrix.CreateTranslation(new Vector3((int)shakeOffset.X, (int)shakeOffset.Y, 0));

            Transform = matrixPos * matrixOffset;
        }
    }
}
