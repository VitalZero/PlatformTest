namespace PlatformTest
{
    public enum TileType:byte { None, Solid, OneWay, Slope }

    public struct Tile
    {
        int tile;
        TileType type;
    }
}
