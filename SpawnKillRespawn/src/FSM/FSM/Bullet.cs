using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FSM
{
    class Bullet : GameObject
    {
        Vector2 direction;
        public Bullet(Texture2D texture, Vector2 position, Vector2 direction, Color color, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.color = color;
            this.layerDepth = 0.8f;
            if (direction.Y > 0)
                rotation = (float)Math.Acos(Vector2.Dot(new Vector2(1, 0), direction));
            else
                rotation = (float)Math.Acos(Vector2.Dot(new Vector2(-1, 0), direction));
        }
        public override void Update(GameTime gameTime)
        {
            position += direction*speed;

        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(boundingBox.X+16, boundingBox.Y+16, boundingBox.Width, boundingBox.Height), texture.Bounds, color, (float)rotation, new Vector2(16, 16), SpriteEffects.None, layerDepth);
        }
    }
}
