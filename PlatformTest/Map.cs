using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PlatformTest
{
    public class Map
    {
        private Tile[] map;
        public int mapWidth;
        public int mapHeight;
        private int tileSize;
        private int textureColumns;
        private Texture2D texture;
        private string textureName;
        ContentManager content;
        BouncingTile bouncingTile;
        private int tileIndexRestore = -1;

        private static Map instance = null;
        public static Map Instance
        {
            get
            {
                return instance;
            }
        }

        public Map()
        {
            instance = this;
        }

        public void Initialize(string directory)
        {
            int[] indexMap;

            try
            {
                JsonDocument mapInfo = JsonDocument.Parse(File.ReadAllText(directory + "\\stage1.json"));

                var mapLayers = mapInfo.RootElement.GetProperty("layers");

                mapWidth = mapLayers[0].GetProperty("width").GetInt32();
                mapHeight = mapLayers[0].GetProperty("height").GetInt32();

                indexMap = new int[mapWidth * mapHeight];
                tileSize = mapInfo.RootElement.GetProperty("tilewidth").GetInt32();

                for(int i = 0; i < (mapWidth * mapHeight); ++i)
                {
                    indexMap[i] = mapLayers[0].GetProperty("data")[i].GetInt32();
                }
            }
            catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            try
            {
                JsonDocument doc = JsonDocument.Parse(File.ReadAllText(directory + "\\stage1tiles.json"));

                textureName = doc.RootElement.GetProperty("image").GetString();
                textureColumns = doc.RootElement.GetProperty("columns").GetInt32();
                map = new Tile[mapWidth * mapHeight];

                for (int y = 0; y < mapHeight; ++y)
                {
                    for (int x = 0; x < mapWidth; ++x)
                    {
                        int tileId = indexMap[x + mapWidth * y] - 1;

                        map[x + mapWidth * y] = new Tile();
                        map[x + mapWidth * y].X = x;
                        map[x + mapWidth * y].Y = y;

                        if (tileId >= 0)
                        {
                            map[x + mapWidth * y].id = tileId;
                            map[x + mapWidth * y].collision = (TileCollision)doc.RootElement.GetProperty("tiles")[tileId].GetProperty("collision").GetInt32();
                        }
                    }
                }

                doc.Dispose();
            }
            catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public void Load(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");
            texture = content.Load<Texture2D>(textureName);
        }

        public void Update(GameTime gameTime)
        {
            if (bouncingTile != null)
            {
                bouncingTile.Update(gameTime);
                if (bouncingTile.Done)
                {
                    map[tileIndexRestore].id = 8;
                    map[tileIndexRestore].collision = TileCollision.solid;
                    bouncingTile = null;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            int xStart = (int)Math.Max(0, ((int)Camera.Instance.XOffset) / tileSize);
            int xEnd = (int)Math.Min(mapWidth, ((int)Camera.Instance.XOffset + (480) / tileSize) + 1);

            for (int y = 0; y < mapHeight; ++y)
            {
                for(int x = xStart; x < xEnd; ++x)
                {
                    int tileTexture = map[x + mapWidth * y].id;

                    if (tileTexture >= 0)
                    {
                        int dx = (tileTexture % textureColumns) * tileSize;
                        int dy = (tileTexture / textureColumns) * tileSize;

                        spriteBatch.Draw(
                            texture,
                            new Vector2((x * tileSize) - (int)Camera.Instance.XOffset, (y * tileSize) - (int)Camera.Instance.YOffset),
                            new Rectangle(dx, dy, tileSize, tileSize),
                            Color.White     
                            );
                    }
                }
            }

            if (bouncingTile != null && bouncingTile.Active)
            {
                int tx = (bouncingTile.TextureID % textureColumns) * tileSize;
                int ty = (bouncingTile.TextureID / textureColumns) * tileSize;

                spriteBatch.Draw(texture,
                    new Vector2((int)bouncingTile.X - (int)Camera.Instance.XOffset, (int)bouncingTile.Y - (int)Camera.Instance.YOffset),
                    new Rectangle(tx, ty, tileSize, tileSize),
                    Color.White);
            }
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 ||
                x >= mapWidth ||
                y < 0 ||
                y >= mapHeight)
                return new Tile();

             return map[x + mapWidth * y];
        }

        public void usedTileItem(int x, int y)
        {
            Tile t = GetTile(x, y);
            tileIndexRestore = x + mapWidth * y;
            bouncingTile = new BouncingTile(t.id, x * tileSize, y * tileSize);
            RemoveTile(x, y);
            //map[x + mapWidth * y].id = 8;
            //map[x + mapWidth * y].collision = TileCollision.solid;
        }

        public void RemoveTile(int x, int y)
        {
            map[x + mapWidth * y] = new Tile();
            map[x + mapWidth * y].X = x;
            map[x + mapWidth * y].Y = y;
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
        }

        private float SpringForce(float y, float restLen)
        {
            float k = .3f;
            float x = y - restLen;

            return -k * x;
        }
    }
}
