using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyBoid
{
    class Tile
    {
        Vector2 position;
        Texture2D texture;
        public Tile(Vector2 position, Texture2D texture)
        {
            this.position = position;
            this.texture = texture;
        }
        public void Update()
        {
            position.X += Game1.tileSize;
            Edges();
        }
        public void Edges()
        {
            if (position.X > Game1.screenSize.X)
            {
                position.X = -texture.Width;
            }
            else if (position.X < -texture.Width)
            {
                position.X = Game1.screenSize.X + texture.Width;
            }
            else if (position.Y > Game1.screenSize.Y)
            {
                position.Y = -texture.Height;
            }
            else if (position.Y < -texture.Height)
            {
                position.Y = Game1.screenSize.Y + texture.Width;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
