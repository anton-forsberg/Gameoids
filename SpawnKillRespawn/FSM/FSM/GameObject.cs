using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FSM
{
    abstract class GameObject
    {
        protected Texture2D texture;
        public Vector2 position;
        protected float speed;
        protected float layerDepth = 0.5f;
        protected Color color = Color.White;
        protected float rotation = 0f;
        public virtual Rectangle boundingBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, boundingBox, texture.Bounds, color, rotation, Vector2.Zero, SpriteEffects.None, layerDepth);
        }
    }
}
