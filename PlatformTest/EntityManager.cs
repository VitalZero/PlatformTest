﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformTest
{
    public class EntityManager
    {
        static List<Goomba> goombas = new List<Goomba>();
        static List<KoopaTrooper> koopaTroopers = new List<KoopaTrooper>();
        static List<Entity> entities = new List<Entity>();

        public static int Count { get { return entities.Count; } }

        public static void Add(Entity entity)
        {
            entities.Add(entity);

            if (entity is Goomba)
                goombas.Add(entity as Goomba);
            else if (entity is KoopaTrooper)
                koopaTroopers.Add(entity as KoopaTrooper);
        }

        public static void Init()
        {
            foreach (var entity in entities)
            {
                entity.Init();
            }
        }

        public static void Update(GameTime gameTime)
        {

            foreach(var entity in entities)
            {
                entity.Update(gameTime);
            }

            HandleCollisions();
            // clean up after update, remove entities that are not active
            entities = entities.Where(e => e.Active).ToList();
            goombas = goombas.Where(e => e.Active).ToList();
            koopaTroopers = koopaTroopers.Where(e => e.Active).ToList();
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
            foreach(var e in entities)
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
                            if (pAABB.Bottom <= tAABB.Center.Y)
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
        }

        private static void HandleCollisions()
        {
            for(int i = 0; i < goombas.Count; ++i)
            {
                if (goombas[i].CanCollide && Player.Instance.CanCollide)
                {
                    Rectangle penetration;
                    Rectangle pAABB = Player.Instance.GetAABB();
                    Rectangle gAABB = goombas[i].GetAABB();

                    Rectangle.Intersect(ref pAABB, ref gAABB, out penetration);

                    if (penetration != Rectangle.Empty)
                    {
                        // if can kill, generate a hit only if player lands on top of the goomba, adjust player position and make it bounce
                        // otherwise, kill the player
                        if (pAABB.Bottom <= gAABB.Center.Y)
                        {
                            goombas[i].Hit();
                            Player.Instance.Move(0, -penetration.Width);
                            Player.Instance.Bounce();
                            //return;
                        }
                        else
                        {
                            if(goombas[i].CanKill)
                                Player.Instance.Hit();

                            return;
                        }
                    }
                }

            }

            for (int i = 0; i < koopaTroopers.Count; ++i)
            {
                if (koopaTroopers[i].CanCollide && Player.Instance.CanCollide)
                {
                    Rectangle penetration;
                    Rectangle pAABB = Player.Instance.GetAABB();
                    Rectangle tAABB = koopaTroopers[i].GetAABB();

                    Rectangle.Intersect(ref pAABB, ref tAABB, out penetration);

                    if (penetration != Rectangle.Empty)
                    {
                        // if cant kill (ie stomped), generate a hit as soon as player touches the koopatropper
                        // adjust koopatrooper position and velocity based on penetration, player position
                        if (!koopaTroopers[i].CanKill)
                        {
                            if (pAABB.Left > tAABB.Left)
                            {
                                koopaTroopers[i].Move(-penetration.Width, 0);
                                koopaTroopers[i].SetDir(-1);
                            }
                            else if (pAABB.Right < tAABB.Right)
                            {
                                koopaTroopers[i].Move(penetration.Width, 0);
                                koopaTroopers[i].SetDir(1);
                            }

                            koopaTroopers[i].Hit();
                        }
                        else
                        {
                            // if can kill, generate a hit only if player lands on top of the koopatrooper
                            // adjust player position and make it bounce
                            // otherwise, kill the player (including when the shell is rebounding)
                            if (pAABB.Bottom <= tAABB.Center.Y)
                            {
                                koopaTroopers[i].Hit();
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
        }
    }
}
