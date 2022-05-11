using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Animation
    {
        private List<Frame> frames;
        private int currentFrameIndex;
        public bool AnimationEnded { get; private set; }
        public Texture2D Texture { get; private set; }
        public bool IsLooping { get; private set; }
        public int FrameWidth { get { return frames[currentFrameIndex].frameRect.Width; }  }
        public int FrameHeight { get { return frames[currentFrameIndex].frameRect.Height; } }
        public int FrameCount { get { return frames.Count; } }
        public int StartFrameX { get { return frames[currentFrameIndex].frameRect.X; } }
        public int StartFrameY { get { return frames[currentFrameIndex].frameRect.Y; } }
        public Frame CurrentFrame { get { return frames[currentFrameIndex]; } }

        public Animation(Texture2D texture)
        {
            Texture = texture;
            currentFrameIndex = 0;
            frames = new List<Frame>();
            AnimationEnded = false;
            IsLooping = true;
        }

        public Animation(
            Texture2D texture,
            float frameTime,
            bool isLooping,
            int frameWidth,
            int frameHeight,
            int frameCount,
            int startFrameX,
            int startFrameY)
        {
            frames = new List<Frame>();
            Texture = texture;
            AnimationEnded = false;
            currentFrameIndex = 0;
            IsLooping = isLooping;

            Frame tmpFrame;

            for (int i = 0; i < frameCount; ++i)
            {
                tmpFrame = new Frame(new Rectangle(startFrameX + (i * frameWidth), startFrameY, frameWidth, frameHeight), frameTime, SpriteEffects.None, null);
                frames.Add(tmpFrame);
                tmpFrame = null;
            }
        }

        public void Advance()
        {
            currentFrameIndex++;

            if (!IsLooping && currentFrameIndex >= frames.Count)
            {
                AnimationEnded = true;
                currentFrameIndex = Math.Min(currentFrameIndex, frames.Count - 1);
            }
            else
            {
                currentFrameIndex = currentFrameIndex % frames.Count;
                AnimationEnded = false;
            }
        }

        public void AddFrame(int x, int y, int width, int height, float frameTime, SpriteEffects flip = SpriteEffects.None, Rectangle? aabb = null)
        {
            Frame tmpFrame = new Frame(new Rectangle(x, y, width, height), frameTime, flip, aabb);
            frames.Add(tmpFrame);
        }

        public void SetSpeed(float frameTime)
        {
            //foreach(var f in frames)
            //{
            //    f.frameTime = Math.Abs(frameTime);
            //}
        }
    }
    //public class Animation
    //{
    //    Texture2D texture;
    //    public Texture2D Texture { get { return texture; } }

    //    float frameTime;
    //    public float FrameTime { get { return frameTime; } }

    //    bool isLooping;
    //    public bool IsLooping { get { return isLooping; } }

    //    int frameWidth;
    //    public int FrameWidth { get { return frameWidth; } }

    //    int frameHeight;
    //    public int FrameHeight { get { return frameHeight; } }

    //    int frameCount;
    //    public int FrameCount { get { return frameCount; } }

    //    int startFrameX;
    //    public int StartFrameX { get { return startFrameX; } }

    //    int startFrameY;
    //    public int StartFrameY { get { return startFrameY; } }

    //    public Animation(
    //        Texture2D texture, 
    //        float frameTime, 
    //        bool isLooping, 
    //        int frameWidth, 
    //        int frameHeight, 
    //        int frameCount, 
    //        int startFrameX, 
    //        int startFrameY)
    //    {
    //        this.texture = texture;
    //        this.frameTime = Math.Abs(frameTime);
    //        this.isLooping = isLooping;
    //        this.frameCount = frameCount;
    //        this.frameWidth = frameWidth;
    //        this.startFrameX = startFrameX;
    //        this.startFrameY = startFrameY;
    //        this.frameHeight = frameHeight;

    //        if (this.texture == null)
    //            throw new NotSupportedException("A valid texture must be provided");
    //    }

    //    public void SetSpeed(float frameTime)
    //    {
    //        this.frameTime = Math.Abs(frameTime);
    //    }
    //}
}
