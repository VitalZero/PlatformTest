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

                            map[x + mapWidth * y].breakable = doc.RootElement.GetProperty("tiles")[tileId].GetProperty("breakable").GetBoolean();
                            map[x + mapWidth * y].solid = doc.RootElement.GetProperty("tiles")[tileId].GetProperty("solid").GetBoolean();
                            if (map[x + mapWidth * y].breakable)
                                map[x + mapWidth * y].collision = TileCollision.breakable;
                            else if (map[x + mapWidth * y].solid && !map[x + mapWidth * y].breakable)
                                map[x + mapWidth * y].collision = TileCollision.solid;
                            else if (!map[x + mapWidth * y].solid && !map[x + mapWidth * y].breakable)
                                map[x + mapWidth * y].collision = TileCollision.none;
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

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int y = 0; y < mapHeight; ++y)
            {
                for(int x = 0; x < mapWidth; ++x)
                {
                    int tileTexture = map[x + mapWidth * y].id;

                    if (tileTexture >= 0)
                    {
                        int dx = (tileTexture % textureColumns) * tileSize;
                        int dy = (tileTexture / textureColumns) * tileSize;

                        spriteBatch.Draw(
                            texture,
                            new Vector2(x * tileSize, y * tileSize),
                            new Rectangle(dx, dy, tileSize, tileSize),
                            Color.White
                            );
                    }
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            return map[x + mapWidth * y];
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
    }
}
