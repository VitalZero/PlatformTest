using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System;
using System.IO;
using VZTiledLoader;
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
        //private string textureName;
        private int tileIndexRestore = -1;
        private Dictionary<int, Item> powerUps;
        private Dictionary<string, List<ObjType>> worldObjects;
        TiledMap tiledMap;
        TileSet tiledSet;
        private List<Area2D> triggerAreas;
        int xStart;
        int xEnd;
        string workingDir;

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
            powerUps = new Dictionary<int, Item>();
            triggerAreas = new List<Area2D>();
            worldObjects = new Dictionary<string, List<ObjType>>();
        }

        public void LoadLevel(string level)
        {
            try
            {
                VZTiledMapLoader mapLoader = new VZTiledMapLoader(level);
                tiledMap = mapLoader.GetObjectMap();

                mapWidth = tiledMap.width;
                mapHeight = tiledMap.height;
                tileSize = tiledMap.tilewidth;


                VZTiledTilesetLoader tileLoader = new VZTiledTilesetLoader("Content\\Levels\\" + tiledMap.tilesetInfo.source);
                tiledSet = tileLoader.GetTileSet();

                textureColumns = tiledSet.columns;
                map = new Tile[mapWidth * mapHeight];

                int leftCameraToWorldValue = (int)-Camera2D.Instance.Transform.Translation.X;
                int rightCameraToWorldValue = (int)-Camera2D.Instance.Transform.Translation.X + 320;

                xStart = (int)Math.Max(0, leftCameraToWorldValue / tileSize);
                xEnd = (int)Math.Min(mapWidth, (rightCameraToWorldValue / tileSize) + 2);

                LoadWorldObjects();

                for (int y = 0; y < mapHeight; ++y)
                {
                    for (int x = 0; x < mapWidth; ++x)
                    {
                        int tileIndex = x + mapWidth * y;
                        int tileId = tiledMap.layer.map[tileIndex] - 1;

                        map[tileIndex] = new Tile();
                        map[tileIndex].X = x;
                        map[tileIndex].Y = y;

                        if (tileIndex == 1885)
                        {
                            float i = 0;
                        }

                        if (tileId >= 0)
                        {
                            if (tileIndex == (55 + mapWidth * 8))
                                map[tileIndex].Visible = false;


                            map[tileIndex].id = tileId;

                            if (powerUps.ContainsKey(tileIndex))
                                map[tileIndex].collision = TileCollision.item;
                            else
                                map[tileIndex].collision = (TileCollision)tiledSet.tiles[tileId].type;
                        }
                    }
                };
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        private void LoadWorldObjects()
        {
            // get all world objects (items, enemies, areas)
            foreach (var p in tiledMap.objectGroups)
            {
                worldObjects.Add(p.name, p.objects);
            }

            //add trigger areas
            if (worldObjects.ContainsKey("areas"))
            {
                foreach (var p in worldObjects["areas"])//tiledMap.objectGroups[2].objects)
                {
                    triggerAreas.Add(new Area2D(p.x, p.y, p.width, p.height, p.type));
                }
            }

            // add items
            if (worldObjects.ContainsKey("items"))
            {
                foreach (var p in worldObjects["items"]) //tiledMap.objectGroups[0].objects)
                {
                    int xTile = (int)(p.x / 16);
                    int yTile = (int)(p.y / 16);

                    int index = xTile + mapWidth * yTile;

                    if (p.type == (int)ItemType.mushroom)
                    {
                        powerUps.Add(index, new Mushroom(new Vector2(xTile * 16, yTile * 16)));
                    }
                    else if (p.type == (int)ItemType.flower)
                    {
                        powerUps.Add(index, new Flower(new Vector2(xTile * 16, yTile * 16)));
                    }
                    else if (p.type == (int)ItemType.star)
                    {
                        powerUps.Add(index, new Star(new Vector2(xTile * 16, yTile * 16)));
                    }
                    else if (p.type == (int)ItemType.oneup)
                    {
                        powerUps.Add(index, new OneUp(new Vector2(xTile * 16, yTile * 16)));
                    }
                    else if (p.type == (int)ItemType.coinBox)
                    {
                        powerUps.Add(index, new CoinBox(new Vector2(xTile * 16, yTile * 16)));
                    }
                }
            }

            //add coins
            if (worldObjects.ContainsKey("coins"))
            {
                foreach (var p in worldObjects["coins"])
                {
                    EntityManager.Add(new Coin(new Vector2(p.x, p.y)));
                }
            }
        }

        public void Initialize(string directory)
        {
            workingDir = directory;

            LoadLevel(directory + "\\Levels\\stage1.tmx");
        }

        public void Load()
        {
            texture = TextureManager.Level;

            if (tiledMap.objectGroups.Count >= 3)
            {
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

            SpriteManager.Add(new StaticSprite(
                new Vector2(3016, 48),
                new Vector2(16, 16),
                new Vector2(0, 0),
                TextureManager.MiscSprites));
        }

        public List<Area2D> GetTriggerAreas()
        {
            return triggerAreas;
        }

        public void Update(GameTime gameTime)
        {
            int leftCameraToWorldValue = (int)-Camera2D.Instance.Transform.Translation.X;
            int rightCameraToWorldValue = (int)-Camera2D.Instance.Transform.Translation.X + 320;

            xStart = (int)Math.Max(0, leftCameraToWorldValue  / tileSize);
            xEnd = (int)Math.Min(mapWidth, (rightCameraToWorldValue / tileSize) + 2);

            if (EntityManager.BouncingTile != null)
            {
                EntityManager.BouncingTile.Update(gameTime);

                if (EntityManager.BouncingTile.Done)
                {
                    if (powerUps.ContainsKey(tileIndexRestore) && powerUps[tileIndexRestore].GetItemType() != ItemType.coinBox)
                    {
                        EntityManager.Add(powerUps[tileIndexRestore]);
                    }

                    Tile t = EntityManager.BouncingTile.Restore();

                    if (EntityManager.BouncingTile.TextureID >= 0)
                    {
                        if (t.collision == TileCollision.item)
                        {
                            map[tileIndexRestore].id = 8;
                            map[tileIndexRestore].Visible = true;
                            map[tileIndexRestore].collision = TileCollision.solid;
                        }
                        else if(t.collision == TileCollision.breakable)
                        {
                            map[tileIndexRestore].Visible = true;
                        }
                    }


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

                    if (tileTexture >= 0 &&
                        map[x + mapWidth * y].Visible  &&
                        !map[x + mapWidth * y].Destroyed)
                    {
                        int dx = (tileTexture % textureColumns) * tileSize;
                        int dy = (tileTexture / textureColumns) * tileSize;

                        spriteBatch.Draw(
                            texture,
                            new Vector2((x * tileSize), (y * tileSize)),
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
                    new Vector2((int)EntityManager.BouncingTile.Position.X, (int)EntityManager.BouncingTile.Position.Y),
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

        public void HitTile(int x, int y, bool canBreak)
        {
            Tile t = GetTile(x, y);

            if(t.collision == TileCollision.breakable)
            {
                if (canBreak)
                {
                    SoundManager.BrickBreak.Play();
                    Camera2D.Instance.Shake();

                    // add brick debris
                    StaticSprite brickDebris = new StaticSprite(
                        new Vector2(x * tileSize, y * tileSize),
                        new Vector2(8, 8),
                        new Vector2(0, 16),
                        TextureManager.MiscSprites);
                    brickDebris.SetAcceleration(new Vector2(0, 850f));
                    brickDebris.SetVelocity(new Vector2(-100f, -150f));
                    SpriteManager.Add(brickDebris);

                    brickDebris = new StaticSprite(
                        new Vector2((x * tileSize) + 8f, y * tileSize),
                        new Vector2(8, 8),
                        new Vector2(0, 16),
                        TextureManager.MiscSprites);
                    brickDebris.SetAcceleration(new Vector2(0, 850f));
                    brickDebris.SetVelocity(new Vector2(100f, -150f));
                    SpriteManager.Add(brickDebris);

                    brickDebris = new StaticSprite(
                        new Vector2(x * tileSize, (y * tileSize) + 8f),
                        new Vector2(8, 8),
                        new Vector2(0, 16),
                        TextureManager.MiscSprites);
                    brickDebris.SetAcceleration(new Vector2(0, 850f));
                    brickDebris.SetVelocity(new Vector2(-60f, -150f));
                    SpriteManager.Add(brickDebris);

                    brickDebris = new StaticSprite(
                        new Vector2((x * tileSize) + 8f, (y * tileSize) + 8f),
                        new Vector2(8, 8),
                        new Vector2(0, 16),
                        TextureManager.MiscSprites);
                    brickDebris.SetAcceleration(new Vector2(0, 850f));
                    brickDebris.SetVelocity(new Vector2(60f, -150f));
                    SpriteManager.Add(brickDebris);

                    DestroyTile(x, y);
                }
                else
                {
                    tileIndexRestore = x + mapWidth * y;
                    map[x + mapWidth * y].Visible = false;
                    EntityManager.BouncingTile = new BouncingTile(t);
                    SoundManager.Thump.Play();
                }
            }
            else if(t.collision == TileCollision.item)
            {
                tileIndexRestore = x + mapWidth * y;
                map[x + mapWidth * y].Visible = false;
                EntityManager.BouncingTile = new BouncingTile(t);

                if (powerUps.ContainsKey(tileIndexRestore))
                {
                    if (powerUps[tileIndexRestore].GetItemType() != ItemType.coinBox)
                    {
                        SoundManager.PowerUp.Play();
                    }
                    else 
                    {
                        EntityManager.Add(powerUps[tileIndexRestore]);
                        SoundManager.CoinBox.Play();
                    }
                }
            }
            else if(t.collision == TileCollision.solid)
            {
                SoundManager.Thump.Play();
            }
        }

        private void DestroyTile(int x, int y)
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
