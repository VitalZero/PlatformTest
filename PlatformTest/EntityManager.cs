﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class EntityManager
    {
        static List<Goomba> goombas = new List<Goomba>();
        static List<Entity> entities = new List<Entity>();
        static List<Entity> entitiesToDelete = new List<Entity>();

        public static int Count { get { return entities.Count; } }

        public static void Add(Entity entity)
        {
            entities.Add(entity);

            if (entity is Goomba)
                goombas.Add(entity as Goomba);
        }

        public static void Update(GameTime gameTime)
        {

            foreach(var entity in entities)
            {
                entity.Update(gameTime);
            }
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
                Rectangle penetration;
                Rectangle pAABB = Player.Instance.GetAABB();
                Rectangle gAABB = goombas[i].GetAABB();

                Rectangle.Intersect(ref pAABB, ref gAABB, out penetration);

                if(penetration != Rectangle.Empty)
                {
                    if(penetration.Height < penetration.Width)
                    {
                        Player.Instance.Kill();
                        break;
                    }
                    else
                    {
                        goombas[i].Kill();
                        break;
                    }
                }

            }
        }
    }
}
