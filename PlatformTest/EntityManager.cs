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
        static List<Entity> entities = new List<Entity>();
        static List<Entity> addedEntities = new List<Entity>();
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
        }

        public static void Update(GameTime gameTime)
        {
            isUpdating = true;

            foreach(var entity in entities)
            {
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

            // clean up after update, remove entities that are not active

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
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

                if (e.CanCollide && Player.Instance.CanCollide)
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
                    p.Collected();
                }
            }
        }

        //private static void HandleCollisions()
        //{
        //    for(int i = 0; i < goombas.Count; ++i)
        //    {
        //        if (goombas[i].CanCollide && Player.Instance.CanCollide)
        //        {
        //            Rectangle penetration;
        //            Rectangle pAABB = Player.Instance.GetAABB();
        //            Rectangle gAABB = goombas[i].GetAABB();

        //            Rectangle.Intersect(ref pAABB, ref gAABB, out penetration);

        //            if (penetration != Rectangle.Empty)
        //            {
        //                // if can kill, generate a hit only if player lands on top of the goomba, adjust player position and make it bounce
        //                // otherwise, kill the player
        //                if (pAABB.Bottom <= gAABB.Center.Y)
        //                {
        //                    goombas[i].Hit();
        //                    Player.Instance.Move(0, -penetration.Width);
        //                    Player.Instance.Bounce();
        //                    //return;
        //                }
        //                else
        //                {
        //                    if(goombas[i].CanKill)
        //                        Player.Instance.Hit();

        //                    return;
        //                }
        //            }
        //        }

        //    }

        //    for (int i = 0; i < koopaTroopers.Count; ++i)
        //    {
        //        if (koopaTroopers[i].CanCollide && Player.Instance.CanCollide)
        //        {
        //            Rectangle penetration;
        //            Rectangle pAABB = Player.Instance.GetAABB();
        //            Rectangle tAABB = koopaTroopers[i].GetAABB();

        //            Rectangle.Intersect(ref pAABB, ref tAABB, out penetration);

        //            if (penetration != Rectangle.Empty)
        //            {
        //                // if cant kill (ie stomped), generate a hit as soon as player touches the koopatropper
        //                // adjust koopatrooper position and velocity based on penetration, player position
        //                if (!koopaTroopers[i].CanKill)
        //                {
        //                    if (pAABB.Left > tAABB.Left)
        //                    {
        //                        koopaTroopers[i].Move(-penetration.Width, 0);
        //                        koopaTroopers[i].SetDir(-1);
        //                    }
        //                    else if (pAABB.Right < tAABB.Right)
        //                    {
        //                        koopaTroopers[i].Move(penetration.Width, 0);
        //                        koopaTroopers[i].SetDir(1);
        //                    }

        //                    koopaTroopers[i].Hit();
        //                }
        //                else
        //                {
        //                    // if can kill, generate a hit only if player lands on top of the koopatrooper
        //                    // adjust player position and make it bounce
        //                    // otherwise, kill the player (including when the shell is rebounding)
        //                    if (pAABB.Bottom <= tAABB.Center.Y)
        //                    {
        //                        koopaTroopers[i].Hit();
        //                        Player.Instance.Move(0, -penetration.Height);
        //                        Player.Instance.Bounce();
        //                        //return;
        //                    }
        //                    else
        //                    {
        //                        Player.Instance.Hit();
        //                        return;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //}
    }
}
