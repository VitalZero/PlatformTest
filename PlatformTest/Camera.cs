using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Camera
    {
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        private Platformer game;
        private static Camera instance = null;

        public static Camera Instance
        {
            get
            {
                if (instance == null)
                    instance = new Camera();

                return instance;
            }
        }

        public void Init(Platformer game, float xOffset, float yOffset)
        {
            XOffset = xOffset;
            YOffset = yOffset;
            this.game = game;
        }

        public void Move(float xAmount, float yAmount) // not used atm
        {
            XOffset += xAmount;
            YOffset += yAmount;
        }

        public static Vector2 WorldToScreen(Vector2 pos)
        {
            pos.X -= instance.XOffset;
            pos.Y -= instance.YOffset;

            return pos;
        }

        public void CenterOnPlayer()
        {
            XOffset = Player.Instance.Position.X - game.Width / 2;
            YOffset = Player.Instance.Position.Y - game.Height / 2;

            XOffset = Math.Clamp(XOffset, 0, (200*16) - game.Width); // clamp to map area
            YOffset = 0; // should not move on y
        }
    }
}
