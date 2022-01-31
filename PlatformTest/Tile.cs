
namespace PlatformTest
{
    public enum TileCollision { none, solid, breakable }

    public class Tile
    {
        public int id = -1;
        public bool breakable = false;
        public bool solid = false;
        public TileCollision collision = TileCollision.none;
        public int X;
        public int Y;
    }
}
