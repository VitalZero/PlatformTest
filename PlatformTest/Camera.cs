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
        private Game1 game;

        public Camera(Game1 game, float xOffset, float yOffset)
        {
            XOffset = xOffset;
            YOffset = yOffset;
            this.game = game;
        }

        public void Move(float xAmount, float yAmount)
        {
            XOffset += xAmount;
            YOffset += yAmount;
        }

        public void CenterOnPlayer(Player player)
        {
            XOffset = player.Pos.X - game.Width / 2;
            YOffset = player.Pos.Y - game.Height / 2;
        }
    }
}
