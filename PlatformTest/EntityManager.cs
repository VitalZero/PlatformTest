using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformTest
{
    public class EntityManager
    {
        static List<Enemy> enemies = new List<Enemy>();
        static List<PowerUp> powerUps = new List<PowerUp>();
        public static BouncingTile BouncingTile { get; set; }
        static List<FireBall> fireBalls = new List<FireBall>();
        static List<Entity> entities = new List<Entity>();
        static List<Entity> addedEntities = new List<Entity>();
        static List<Entity> drawBehind = new List<Entity>();
        static List<Entity> drawNormal = new List<Entity>();

        static bool isUpdating;

        public static int Count { get { return entities.Count; } }

        public static void Add(Entity entity)
        {
            if(!isUpdating)
            {
                AddEntity(entity);
            }
            else
            {
                addedEntities.Add(entity);
            }
        }

        private static void AddEntity(Entity entity)
        {
            entity.Init();
            entities.Add(entity);

            if (entity is Enemy)
                enemies.Add(entity as Enemy);
            else if (entity is PowerUp)
                powerUps.Add(entity as PowerUp);
            else if (entity is FireBall)
                fireBalls.Add(entity as FireBall);
        }

        public static int FireBallCount()
        {
            return fireBalls.Count;
        }

        public static void Init()
        {   
            foreach (var entity in entities)
            {
                entity.Init();
            }
        }

        public static void RemoveInactiveEntities()
        {
            entities = entities.Where(e => !e.Destroyed).ToList();
            enemies = enemies.Where(e => !e.Destroyed).ToList();
            powerUps = powerUps.Where(e => !e.Destroyed).ToList();
            fireBalls = fireBalls.Where(e => !e.Destroyed).ToList();
        }

        public static void CheckForEnemiesAndActivate(int index)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.Index == index)
                    enemy.Active = true;
            }
        }

        public static void Update(GameTime gameTime)
        {
            isUpdating = true;

            foreach(var entity in entities)
            {
                if(entity.Active)
                    entity.Update(gameTime);
            }

            //HandleCollisions();
            CollideAndResolveBetweenEntities();

            isUpdating = false;

            foreach(var entity in addedEntities)
            {
                AddEntity(entity);
            }

            isUpdating = false;

            RemoveInactiveEntities();

            addedEntities.Clear();

            drawBehind = entities.Where(e => e.DrawBehind).ToList();
            drawNormal = entities.Where(e => !e.DrawBehind).ToList();
            drawNormal.Sort((e1, e2) => (e1.drawPriority.CompareTo(e2.drawPriority)));

            // clean up after update, remove entities that are not active

        }

        public static void DrawBehind(SpriteBatch spriteBatch)
        {
            foreach (var entity in drawBehind)
            {
                entity.Draw(spriteBatch);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in drawNormal)
            {
                entity.Draw(spriteBatch);
            }
        }

        private static bool IsColliding(Entity a, Entity b)
        {
            Rectangle aabb1 = a.GetAABB();
            Rectangle aabb2 = b.GetAABB();

            return aabb1.Intersects(aabb2);
        }

        private static void CollideAndResolveBetweenEntities()
        {
            // enemies collision
            foreach(var e in enemies)
            {
                if (Player.Instance.Equals(e))
                    continue;

                // collision if a tile under the enemy was hit
                if (BouncingTile != null)
                {
                    Rectangle bAABB;
                    Rectangle penetration;
                    Rectangle eAABB = e.GetAABB();

                    bAABB = BouncingTile.GetAABB();

                    if (e.CanCollide && e.CanKill)
                    {
                        Rectangle.Intersect(ref bAABB, ref eAABB, out penetration);

                        if (penetration != Rectangle.Empty)
                        {
                            e.Kill();
                        }
                    }
                }

                // enemy vs fireballs
                if (e.CanCollide)
                {
                    foreach (var f in fireBalls)
                    {
                        Rectangle penetration;
                        Rectangle fAABB = f.GetAABB();
                        Rectangle eAABB = e.GetAABB();

                        Rectangle.Intersect(ref fAABB, ref eAABB, out penetration);

                        if (penetration != Rectangle.Empty)
                        {
                            e.Kill();
                            f.Destroy();
                        }
                    }
                }

                    // enemy vs player collision
                    if (e.CanCollide && Player.Instance.CanCollide && e.Active)
                {
                    Rectangle penetration;
                    Rectangle pAABB = Player.Instance.GetAABB();
                    Rectangle tAABB = e.GetAABB();
                    
                    Rectangle.Intersect(ref pAABB, ref tAABB, out penetration);

                    if (penetration != Rectangle.Empty)
                    {
                        // if cant kill (ie stomped), generate a hit as soon as player touches the koopatropper
                        // adjust koopatrooper position and velocity based on penetration, player position
                        if (!e.CanKill)
                         {
                            if (pAABB.Left > tAABB.Left)
                            {
                                e.Move(-penetration.Width, 0);
                                e.SetDir(-1);
                            }
                            else if (pAABB.Right < tAABB.Right)
                            {
                                e.Move(penetration.Width, 0);
                                e.SetDir(1);
                            }

                            e.Hit();
                        }
                        else
                        {
                            // if can kill, generate a hit only if player lands on top of the koopatrooper
                            // adjust player position and make it bounce
                            // otherwise, kill the player (including when the shell is rebounding)
                            if (pAABB.Bottom <= tAABB.Center.Y ||
                                (int)Player.Instance.PrevPos.Y < (int)Player.Instance.Pos.Y)
                            {
                                e.Hit();
                                Player.Instance.Move(0, -penetration.Height);
                                Player.Instance.Bounce();
                                //return;
                            }
                            else
                            {
                                Player.Instance.Hit();
                                return;
                            }
                        }
                    }
                }
            }

            // powerups collision
            foreach (var p in powerUps)
            {
                Rectangle penetration;
                Rectangle pAABB = Player.Instance.GetAABB();
                Rectangle tAABB = p.GetAABB();

                Rectangle.Intersect(ref pAABB, ref tAABB, out penetration);

                if (penetration != Rectangle.Empty)
                {
                    if(p is Mushroom)
                    {
                        Player.Instance.Grow();
                    }
                    if (p is Flower)
                    {
                        Player.Instance.Burn();
                    }

                    p.Collected();
                }
            }
        }
    }
}
