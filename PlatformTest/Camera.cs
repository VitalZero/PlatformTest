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

        public Camera(Platformer game, float xOffset, float yOffset)
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

        public void CenterOnPlayer(Player player)
        {
            XOffset = player.Pos.X - game.Width / 2;
            YOffset = player.Pos.Y - game.Height / 2;

            XOffset = Math.Clamp(XOffset, 0, (71*16) - game.Width); // clamp to map area
            YOffset = 0; // should not move on y
        }
    }
}
