using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBoid
{
    class Sparrow : Bird
    {
        float cohesion = 0.025f;
        float alignment = 0.5f;
        new float separation;
        float fearOfRavens = 1.1f;

        public Sparrow(Vector2 boundary, Flock flock, Texture2D texture)
            : base(boundary, texture)
        {
            this.flock = flock;
            speed = 6f;
            separation = texture.Width;
        }

        public override void Update(GameTime gameTime)
        {
            Flock();

            base.Update(gameTime);
        }

        private void Flock()
        {
            foreach (Bird bird in flock.Birds.Where(b => b is Sparrow && b != this))
            {
                float distance = Vector2.Distance(position, bird.Position);
 
                if (distance < separation)
                {
                    //separation
                    //influence the movement direction vector away from the nearby sparrow
                    direction += position - bird.Position;
                }
                else if (distance < sight)
                {
                    //cohesion
                    //influence the movement direction vector towards the sighted sparrow
                    direction += (bird.Position - position) * cohesion;
                }
 
                if (distance < sight)
                {
                    //alignment
                    //influence the movement direction vector in the direction the sighted sparrow is moving
                    direction += bird.Offset * alignment;
                }
            }

            foreach (Bird raven in flock.Birds.Where(b => (b is Raven || b is Eagle) && b != this))
            {
                float distanceRaven = Vector2.Distance(position, raven.Position);
                if (distanceRaven < sight) // Flee
                {
                    //influence the movement direction vector away from the sighted Raven
                    direction += (position - raven.Position) * fearOfRavens;
                }
            }       
    }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //if (KeyMouseReader.IsKeyDown(Keys.LeftControl))
            //    spriteBatch.Draw(Game1.circle, new Rectangle((int)(position.X - sight), (int)(position.Y - sight), (int)sight * 2, (int)sight * 2), Color.Blue);
            spriteBatch.Draw(texture, position, texture.Bounds, Color.White, rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, flip, 1f);
        }
    }
}
