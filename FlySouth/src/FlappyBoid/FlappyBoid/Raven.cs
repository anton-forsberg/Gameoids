using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBoid
{
    class Raven : Bird
    {
        public Raven(Vector2 boundary, Flock flock, Texture2D texture)
            : base(boundary, texture)
        {
            this.flock = flock;
            speed = 4f;
            sight = 90;
        }

        public override void Update(GameTime gameTime)
        {
            Hunt();
            Separate();
            if (CollisionCheck<Sparrow>(gameTime))
            {
                //make the raven more powerful when it eats sparrows
                sight += 2;
                speed += 0.1f;
            }
            
            base.Update(gameTime);
        }

        private void Separate()
        {
            foreach (Bird bird in flock.Birds.Where(b => b is Raven && b != this))
            {
                float distance = Vector2.Distance(position, bird.Position);

                if (distance < separation)
                {
                    //separation
                    //influence the movement direction vector away from the nearby Raven
                    direction += position - bird.Position;
                }
            }
        }

        private void Hunt()
        {
            //find the closest sparrow within sight
            var l = flock.Birds.Where(b => b is Sparrow && Vector2.Distance(position, b.Position) < sight);
            var m = l.OrderBy(b => Vector2.Distance(position, b.Position));
            var n = m.FirstOrDefault();
            Bird p = ((Bird)n);
 
            // and influence the movement direction towards the sparrow to attack...
            if (p != null)
                direction += p.Position - position;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (KeyMouseReader.IsKeyDown(Keys.LeftControl))
                spriteBatch.Draw(Game1.circle, new Rectangle((int)(position.X - sight), (int)(position.Y - sight), (int)sight * 2, (int)sight * 2), Color.White);
            spriteBatch.Draw(texture, position, texture.Bounds, Color.White, rotation, new Vector2(texture.Width/2, texture.Height/2), 1f, flip, 1f);
        }
    }
}
