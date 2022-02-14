using System;

namespace PlatformTest
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Platformer())
                game.Run();
        }
    }
}
