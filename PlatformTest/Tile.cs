
namespace PlatformTest
{
    public enum TileCollision { none, solid, breakable, item }

    public class Tile
    {
        public int size;
        public int id;
        public TileCollision collision;
        public bool Visible { get; set; }
        public bool Destroyed { get; set; }
        public int X;
        public int Y;
        public int itemQty = 0;

        public Tile()
        {
            id = -1;
            size = 16;
            collision = TileCollision.none;
            Visible = true;
            Destroyed = false;
        }
    }
}
