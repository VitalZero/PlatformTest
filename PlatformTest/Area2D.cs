using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public enum AreaType { none, downPipe, rightPipe, leftPipe, upPipe, goal = 20, endStage }

    public class Area2D
    {
        private Rectangle area;
        private AreaType type;
        private Vector2 pos;
        public bool Active { get; set; }
        public AreaType Type { get { return type; } }

        public Area2D(float x, float y, float width, float height, int type)
        {
            pos = new Vector2(x, y);
            area = new Rectangle((int)pos.X, (int)pos.Y, (int)width, (int)height);
            this.type = (AreaType)type;
            Active = true;
        }

        public Rectangle GetAABB()
        {
            return new Rectangle((int)pos.X, (int)pos.Y, area.Width, area.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(TextureManager.Pixel,
            //    new Vector2((int)pos.X, (int)pos.Y),
            //        area,
            //        new Color(Color.Red, 0.1f));
        }
    }

}
