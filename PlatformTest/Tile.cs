
namespace PlatformTest
{
    public enum TileCollision { none, solid, breakable, item }

    public class Tile
    {
        public int size = 16;
        public int id = -1;
        public TileCollision collision = TileCollision.none;
        public int X;
        public int Y;
        public int itemQty = 0;
    }
}
