
namespace PlatformTest
{
    public enum TileCollision { none, solid, breakable, item }
    public enum ItemType { none, coin, multicoin, mushroom, flower, oneup }

    public class Tile
    {
        public int size = 16;
        public int id = -1;
        public TileCollision collision = TileCollision.none;
        public int X;
        public int Y;
        public ItemType item = ItemType.none;
        public int itemQty = 0;
    }
}
