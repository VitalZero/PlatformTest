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
        static List<Goomba> goombas = new List<Goomba>();
        static List<Turtle> turtles = new List<Turtle>();
        static List<Entity> entities = new List<Entity>();

        public static int Count { get { return entities.Count; } }

        public static void Add(Entity entity)
        {
            entities.Add(entity);

            if (entity is Goomba)
                goombas.Add(entity as Goomba);
            else if (entity is Turtle)
                turtles.Add(entity as Turtle);
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
            entities = entities.Where(e => e.Active).ToList();
            goombas = goombas.Where(e => e.Active).ToList();
            turtles = turtles.Where(e => e.Active).ToList();
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
                        if (penetration.Height <= penetration.Width &&
                            pAABB.Top < gAABB.Top)
                        {
                            goombas[i].Hit();
                            Player.Instance.Move(0, -penetration.Width);
                            Player.Instance.Bounce();
                            return;
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

            for (int i = 0; i < turtles.Count; ++i)
            {
                if (turtles[i].CanCollide && Player.Instance.CanCollide)
                {
                    Rectangle penetration;
                    Rectangle pAABB = Player.Instance.GetAABB();
                    Rectangle tAABB = turtles[i].GetAABB();

                    Rectangle.Intersect(ref pAABB, ref tAABB, out penetration);

                    if (penetration != Rectangle.Empty)
                    {
                        if (penetration.Height <= penetration.Width &&
                            pAABB.Top < tAABB.Top)
                        //if(pAABB.Bottom < tAABB.Center.Y)
                        {
                            turtles[i].Hit();
                            Player.Instance.Move(0, -penetration.Height);
                            Player.Instance.Bounce();
                            return;
                        }
                        else
                        {
                            if (turtles[i].CanKill) 
                                Player.Instance.Hit();
                            else
                            {
                                if (pAABB.Left > tAABB.Left)
                                {
                                    turtles[i].Move(-penetration.Width, 0);
                                    turtles[i].SetDir(-1);
                                }
                                else if (pAABB.Right < tAABB.Right)
                                {
                                    turtles[i].Move(penetration.Width, 0);
                                    turtles[i].SetDir(1);
                                }

                                turtles[i].Hit();
                            }

                            return;
                        }
                    }
                }

            }
        }
    }
}
