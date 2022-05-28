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
        public static SoundEffect InstantKill { get; set; }
        public static SoundEffect Thump { get; set; }
        public static SoundEffect ShrinkPipe { get; set; }
        public static SoundEffect CoinBox { get; set; }
        public static SoundEffect Coin { get; set; }
        public static SoundEffect Stomp { get; set; }
        public static SoundEffect BrickBreak { get; set; }
        public static SoundEffect OneUpNormal { get; set; }
        public static SoundEffect OneUpMushroom { get; set; }
        public static SoundEffect PoleDown { get; set; }
        public static Song SurfaceStage { get; set; }
        public static Song StarSong { get; set; }

        public static void Load(ContentManager content)
        {
            JumpBig = content.Load<SoundEffect>("Sounds/jumpbig");
            JumpSmall = content.Load<SoundEffect>("Sounds/jumpsmall");
            SurfaceStage = content.Load<Song>("Sounds/smbsurface");
            FireBall = content.Load<SoundEffect>("Sounds/fireball");
            PowerUp = content.Load<SoundEffect>("Sounds/powerup");
            Grow = content.Load<SoundEffect>("Sounds/grow"); 
            InstantKill= content.Load<SoundEffect>("Sounds/instantkill");
            Thump = content.Load<SoundEffect>("Sounds/thump");
            ShrinkPipe = content.Load<SoundEffect>("Sounds/shrink-pipe");
            CoinBox = content.Load<SoundEffect>("Sounds/coinbox");
            Coin = content.Load<SoundEffect>("Sounds/coin");
            Stomp = content.Load<SoundEffect>("Sounds/stomp");
            BrickBreak = content.Load<SoundEffect>("Sounds/brickbreak");
            OneUpNormal = content.Load<SoundEffect>("Sounds/oneupnormal");
            OneUpMushroom = content.Load<SoundEffect>("Sounds/oneupmushroom");
            PoleDown = content.Load<SoundEffect>("Sounds/poledown");
            StarSong = content.Load<Song>("Sounds/starremix");
        }
    }
}

