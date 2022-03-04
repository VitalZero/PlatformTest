﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class CoinBox : PowerUp
    {
        private bool rising;
        private float yOrigin;
        private AnimationPlayer animPlayer;

        public CoinBox(Vector2 pos)
            : base(PowerupType.coin, pos)
        {
            speed = 250f;
            gravity = 20f;
            dir = 0f;
            CanCollide = false;
            this.pos.Y -= 16f;
            this.pos.X += 4f;
            rising = true;

            yOrigin = pos.Y;
        }

        public override void Init()
        {
            texture = ResourceManager.MiscSprites;
            animPlayer = new AnimationPlayer();
            animPlayer.Add("idle", new Animation(texture, 0.04f, true, 8, 16, 4, 0, 16));
            spriteArea = new Rectangle(0, 0, 16, 16);
        }

        public override void Update(GameTime gameTime)
        {
            animPlayer.PlayAnimation("idle");

            startY = 16f;
            yOffset = 0;

            if(rising)
            {
                vel.Y = -speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                rising = false;
            }

            vel.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            pos.Y += vel.Y;

            if(pos.Y >= yOrigin - 16f)
            {
                Destroyed = true;
            }


            animPlayer.Update(gameTime);

            //LateUpdate(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animPlayer.Draw(spriteBatch,
                new Vector2((int)pos.X - (int)Camera.Instance.XOffset, (int)pos.Y - (int)Camera.Instance.YOffset),
                SpriteEffects.None);
        }
    }
}