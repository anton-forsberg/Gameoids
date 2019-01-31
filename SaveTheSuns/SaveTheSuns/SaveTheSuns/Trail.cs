using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SaveTheSuns
{
    class Trail
    {
        Vector2 position;
        Color color;
        Texture2D texture;
        List<Trail> tail;
        public Trail(Vector2 position, List<Trail> tail, Texture2D texture)
        {
            this.position = position;
            color = Color.White;
            this.tail = tail;
            this.texture = texture;
        }

        public void Update()
        {
            color *= 0.9f;
            if (color.A < 10)
                tail.Remove(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Vector2(position.X - texture.Width/2, position.Y - texture.Height/2), color);
        }
    }
}
