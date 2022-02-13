using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformTest
{
    public abstract class GameObject
    {
        protected Texture2D texture;
        protected Vector2 pos;
        protected Vector2 size;
        protected Rectangle aabb;

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
