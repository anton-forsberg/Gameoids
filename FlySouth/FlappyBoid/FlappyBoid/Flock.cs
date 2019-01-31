using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyBoid
{
    public class Flock
{
    public List<Bird> Birds = new List<Bird>();

    public Flock(Vector2 boundary, int nofSparrows, int nofRavens, Texture2D sparrowTexture, Texture2D ravenTexture, Texture2D eagleTexture)
    {
        for (int i = 0; i < nofSparrows; i++)
        {
            Birds.Add(new Sparrow(boundary, this, sparrowTexture));
        }
 
        for (int i = 0; i < nofRavens; i++)
        {
            Birds.Add(new Raven(boundary, this, ravenTexture));
        }

        Birds.Add(new Eagle(boundary, this, eagleTexture));
    }
 
    public void Update(GameTime gameTime)
    {
        foreach (Bird bird in Birds.ToList())
        {
            bird.Update(gameTime);
        }
    }
 
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Bird bird in Birds.ToList())
        {
            bird.Draw(spriteBatch);
        }            
    }
}
}
