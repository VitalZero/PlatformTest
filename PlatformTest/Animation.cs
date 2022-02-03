using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    class Animation
    {
        Texture2D texture;
        public Texture2D Texture { get { return texture; } }
        float frameTime;
        public float FrameTime { get { return frameTime; } }
        bool isLooping;
        public bool IsLooping { get { return isLooping; } }
        int frameWidth;
        public int FrameWidth { get { return frameWidth; } }
        public int FrameHeight { get { return Texture.Height; } }
        int frameCount;
        public int FrameCount { get { return frameCount; } }
        int startFrameX;
        public int StartFrameX { get { return startFrameX; } }
        int startFrameY;
        public int StartFrameY { get { return startFrameX; } }

        public Animation(Texture2D texture, float frameTime, bool isLooping, int frameWidth, int frameCount, int startFrameX, int startFrameY)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            this.frameCount = frameCount;
            this.frameWidth = frameWidth;
            this.startFrameX = startFrameX;
            this.startFrameY = startFrameY;

            if (this.texture == null)
                throw new NotSupportedException("A valid texture must be provided");
        }

    }
}
