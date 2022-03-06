using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System;
using System.IO;
using VZTMXMapLoader;
using System.Collections.Generic;
using System.Linq;

namespace PlatformTest
{
    public class World
    {
        private Tile[] map;
        public int mapWidth;
        public int mapHeight;
        private int tileSize;
        private int textureColumns;
        private Texture2D texture;
        private string textureName;
        BouncingTile bouncingTile;
        private int tileIndexRestore = -1;
        private Dictionary<int, PowerUp> powerUps;
        TiledMap tiledMap;
        int xStart;
        int xEnd;

        private static World instance = null;
        public static World Instance
        {
            get
            {
                return instance;
            }
        }

        public World()
        {
            instance = this;
            powerUps = new Dictionary<int, PowerUp>();
        }

        public void Initialize(string directory)
        {
            int[] indexMap;
            {
                TMXMapLoader loader = new TMXMapLoader(directory + "\\stage1.tmx");

                tiledMap = loader.GetObjectMap();
            }

            mapWidth = tiledMap.width;
            mapHeight = tiledMap.height;
            tileSize = tiledMap.tilewidth;

            foreach(var p in tiledMap.objectGroups[0].objects)
            {
                int xTile = (int)(p.x / 16);
                int yTile = (int)(p.y / 16);

                int index = xTile + mapWidth * yTile;
                
                if(p.type == (int)PowerupType.mushroom)
                {
                    powerUps.Add(index, new Mushroom(new Vector2(xTile * 16, yTile * 16)));
                }
                else if (p.type == (int)PowerupType.flower)
                {
                    powerUps.Add(index, new Flower(new Vector2(xTile * 16, yTile * 16)));
                }
                else if (p.type == (int)PowerupType.star)
                {
                    powerUps.Add(index, new Star(new Vector2(xTile * 16, yTile * 16)));
                }
                else if (p.type == (int)PowerupType.oneup)
                {
                    powerUps.Add(index, new OneUp(new Vector2(xTile * 16, yTile * 16)));
                }
                else if (p.type == (int)PowerupType.coin)
                {
                    powerUps.Add(index, new CoinBox(new Vector2(xTile * 16, yTile * 16)));
                }
            }

            try
            {
                JsonDocument mapInfo = JsonDocument.Parse(File.ReadAllText(directory + "\\map_level1.json"));

                var mapLayers = mapInfo.RootElement.GetProperty("layers");

                //mapWidth = mapLayers[0].GetProperty("width").GetInt32();
                //mapHeight = mapLayers[0].GetProperty("height").GetInt32();

                indexMap = new int[mapWidth * mapHeight];
                //tileSize = mapInfo.RootElement.GetProperty("tilewidth").GetInt32();

                for(int i = 0; i < (mapWidth * mapHeight); ++i)
                {
                    indexMap[i] = mapLayers[0].GetProperty("data")[i].GetInt32();
                }

                mapInfo.Dispose();
            }
            catch(Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            try
            {
                JsonDocument doc = JsonDocument.Parse(File.ReadAllText(directory + "\\tileset.json"));

                textureName = doc.RootElement.GetProperty("image").GetString();
                textureColumns = doc.RootElement.GetProperty("columns").GetInt32();
                map = new Tile[mapWidth * mapHeight];

                for (int y = 0; y < mapHeight; ++y)
                {
                    for (int x = 0; x < mapWidth; ++x)
                    {
                        //int tileId = indexMap[x + mapWidth * y] - 1;
                        int tileId = tiledMap.layer.map[x + mapWidth * y] - 1;

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

        public void Load()
        {
            texture = ResourceManager.Level;

            foreach (var enemyType in tiledMap.objectGroups[1].objects)
            {
                int xSpawn = (int)enemyType.x / 16;
                int ySpawn = (int)enemyType.y / 16;
                int enemyIndex = xSpawn + mapWidth * ySpawn;

                if (enemyType.type == 1)
                {
                    EntityManager.Add(new Goomba(new Vector2(xSpawn * 16, ySpawn * 16), enemyIndex));
                    enemyType.ToDelete = true;
                }
                else if (enemyType.type == 2)
                {
                    EntityManager.Add(new KoopaTrooper(new Vector2(xSpawn * 16, ySpawn * 16), enemyIndex));
                    enemyType.ToDelete = true;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            xStart = (int)Math.Max(0, ((int)Camera.Instance.XOffset) / tileSize);
            xEnd = (int)Math.Min(mapWidth, ((int)(Camera.Instance.XOffset + 320) / tileSize) + 1);


            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = xStart; x < xEnd + 2; ++x)
                {
                    int enemyIndex = x + mapWidth * y;
                    EntityManager.CheckForEnemiesAndActivate(enemyIndex);
                }
            }

            if (bouncingTile != null)
            {
                bouncingTile.Update(gameTime);
                if (bouncingTile.Done)
                {
                    map[tileIndexRestore].id = 8;
                    map[tileIndexRestore].collision = TileCollision.solid;

                    if (powerUps.ContainsKey(tileIndexRestore))
                        EntityManager.Add(powerUps[tileIndexRestore]);
                    //EntityManager.Add(new Mushroom(new Vector2(bouncingTile.X, bouncingTile.Y)));
                    bouncingTile = null;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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
            bouncingTile = new BouncingTile(t);
            DestroyTile(x, y);

            //map[x + mapWidth * y].id = 8;
            //map[x + mapWidth * y].collision = TileCollision.solid;
        }

        private void DestroyTile(int x, int y)
        {
            map[x + mapWidth * y] = new Tile();
            map[x + mapWidth * y].X = x;
            map[x + mapWidth * y].Y = y;
        }

        public void RemoveTile(int x, int y)
        {
            int tmpIndex = x + mapWidth * y;

            if(powerUps.ContainsKey(tmpIndex))
            {
                Tile t = GetTile(x, y);
                tileIndexRestore = tmpIndex;
                bouncingTile = new BouncingTile(t);
                DestroyTile(x, y);
            }
            else
            {
                DestroyTile(x, y);
            }
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
