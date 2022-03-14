using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public static class SoundManager
    {
        public static SoundEffect JumpBig { get; set; }
        public static SoundEffect JumpSmall { get; set; }
        public static SoundEffect FireBall { get; set; }
        public static SoundEffect PowerUp { get; set; }
        public static SoundEffect Grow { get; set; }
        public static Song SurfaceStage { get; set; }

        public static void Load(ContentManager content)
        {
            JumpBig = content.Load<SoundEffect>("Sounds/jumpbig");
            JumpSmall = content.Load<SoundEffect>("Sounds/jumpsmall");
            SurfaceStage = content.Load<Song>("Sounds/smbsurface");
            FireBall = content.Load<SoundEffect>("Sounds/fireball");
            PowerUp = content.Load<SoundEffect>("Sounds/powerup");
            Grow = content.Load<SoundEffect>("Sounds/grow");
        }
    }
}

