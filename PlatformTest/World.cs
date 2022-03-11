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
        private int tileIndexRestore = -1;
        private Dictionary<int, PowerUp> powerUps;
        TiledMap tiledMap;
        private List<Area2D> triggerAreas;
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
            triggerAreas = new List<Area2D>();
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

            foreach (var p in tiledMap.objectGroups[2].objects)
            {
                triggerAreas.Add(new Area2D(p.x, p.y, p.width, p.height, p.type));
            }

            foreach (var p in tiledMap.objectGroups[0].objects)
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

        public List<Area2D> GetTriggerAreas()
        {
            return triggerAreas;
        }

        public void Update(GameTime gameTime)
        {
            xStart = (int)Math.Max(0, (((int)Camera.Instance.XOffset) / tileSize) -1);
            xEnd = (int)Math.Min(mapWidth, ((int)(Camera.Instance.XOffset + 320) / tileSize) + 2);

            if (EntityManager.BouncingTile != null)
            {
                EntityManager.BouncingTile.Update(gameTime);
                if (EntityManager.BouncingTile.Done)
                {
                    if (EntityManager.BouncingTile.TextureID >= 0)
                    {
                        map[tileIndexRestore].id = 8;
                        map[tileIndexRestore].collision = TileCollision.solid;
                    }

                    if (powerUps.ContainsKey(tileIndexRestore))
                        EntityManager.Add(powerUps[tileIndexRestore]);
                    
                    EntityManager.BouncingTile = null;
                }
            }

            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = xStart; x < xEnd; ++x)
                {
                    int index = x + mapWidth * y;
                    EntityManager.CheckForEnemiesAndActivate(index);
                    if (map[index].Destroyed)
                        DestroyTile(x, y);
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

                    if (tileTexture >= 0 && map[x + mapWidth * y].Visible && !map[x + mapWidth * y].Destroyed)
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

            if (EntityManager.BouncingTile != null && EntityManager.BouncingTile.Active && EntityManager.BouncingTile.TextureID >= 0)
            {
                int tx = (EntityManager.BouncingTile.TextureID % textureColumns) * tileSize;
                int ty = (EntityManager.BouncingTile.TextureID / textureColumns) * tileSize;

                spriteBatch.Draw(texture,
                    new Vector2((int)EntityManager.BouncingTile.Position.X - (int)Camera.Instance.XOffset, (int)EntityManager.BouncingTile.Position.Y - (int)Camera.Instance.YOffset),
                    new Rectangle(tx, ty, tileSize, tileSize),
                    Color.White);
            }

            foreach(var a in triggerAreas)
            {
                a.Draw(spriteBatch);
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
            EntityManager.BouncingTile = new BouncingTile(t);
            DestroyTile(x, y);
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
            tileIndexRestore = tmpIndex;

            if (powerUps.ContainsKey(tmpIndex))
            {
                Tile t = GetTile(x, y);
                
                EntityManager.BouncingTile = new BouncingTile(t);
            }
            else
            {
                Tile t = new Tile();
                t.id = -1;
                t.X = x;
                t.Y = y;
                EntityManager.BouncingTile = new BouncingTile(t);
            }

            map[tmpIndex].Destroyed = true;
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
