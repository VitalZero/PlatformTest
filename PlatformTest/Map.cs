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
        Camera camera;
        Tile tileToBounce;
        float bounceTimer = 0f;
        float maxBounceTime = .2f;
        float bounceForce;
        float velY;

        public Map(Camera camera)
        {
            this.camera = camera;
            tileToBounce = new Tile();
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
            if(tileToBounce.id > 0)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                bounceForce = SpringForce(bounceForce, 200f);
                velY += bounceForce * dt;


                if(bounceTimer >= maxBounceTime)
                {
                    tileToBounce = new Tile();
                    bounceTimer = 0f;
                    bounceForce = 0;
                    velY = 0;
                }

                bounceTimer += dt;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            int xStart = (int)Math.Max(0, (camera.XOffset) / tileSize);
            int xEnd = (int)Math.Min(mapWidth, (camera.XOffset + (480) / tileSize) + 1);

            for (int y = 0; y < mapHeight; ++y)
            {
                for(int x = xStart; x < xEnd; ++x)
                {
                    int tileTexture = map[x + mapWidth * y].id;

                    if (tileTexture >= 0)
                    {
                        int dx = (tileTexture % textureColumns) * tileSize;
                        int dy = (tileTexture / textureColumns) *tileSize;

                        spriteBatch.Draw(
                            texture,
                            new Vector2((int)((x * tileSize) - camera.XOffset), (int)((y * tileSize) - camera.YOffset)),
                            new Rectangle(dx, dy, tileSize, tileSize),
                            Color.White
                            );
                    }
                }
            }

            if(tileToBounce.id > 0)
            {
                int dx = (tileToBounce.id % textureColumns) * tileSize;
                int dy = (tileToBounce.id / textureColumns) * tileSize;

                spriteBatch.Draw(
                    texture,
                    new Vector2((int)((tileToBounce.X * tileSize) - camera.XOffset), (int)((tileToBounce.Y * tileSize) - camera.YOffset - velY)),
                    new Rectangle(dx, dy, tileSize, tileSize),
                    Color.White
                    );
            }
        }

        public Tile GetTile(int x, int y)
        {
            if ((x + mapWidth * y) >= map.Length)
                return new Tile();

             return map[x + mapWidth * y];
        }

        public void usedTileItem(int x, int y)
        {
            tileToBounce = GetTile(x, y);
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
