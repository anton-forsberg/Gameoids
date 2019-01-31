using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyBoid
{
    public abstract class Bird
    {
        protected static Random rand = new Random();
        protected static Vector2 border = new Vector2(50f, 50f);
        protected float sight = 75f;
        protected static float separation = 30f;
        protected float speed;
        protected Vector2 boundary;
        protected float rotation;
        protected Flock flock;
        protected Texture2D texture;
        protected SpriteEffects flip;
        
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }
        public Rectangle boundingBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }
        protected Vector2 direction;

        public Vector2 Offset
        {
            get { return direction; }
        }

        public Bird(Vector2 boundary, Texture2D texture)
        {
            position = new Vector2(rand.Next((int)boundary.X), rand.Next((int)boundary.Y));
            this.boundary = boundary;
            this.texture = texture;
        }

        public virtual void Update(GameTime gameTime)
        {
            //Edges();
            HandleEdgeCollision();
            NormalizeOffset();

            //Move the bird using the current direction
            position += direction * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

            rotation = (float)Math.Atan2(direction.Y , direction.X); 
            flip = SpriteEffects.None;
            if (direction.X < 0)
            {
                flip = SpriteEffects.FlipVertically;
            }
        }

        protected void Edges()
        {
            if (position.X - texture.Width / 2 > boundary.X)
            {
                position.X = -texture.Width / 2 + 1;
            }
            else if (position.X + texture.Width / 2 < 0)
            {
                position.X = boundary.X + texture.Width / 2 - 1;
            }
            else if (position.Y - texture.Width / 2 > boundary.Y)
            {
                position.Y = -texture.Width / 2 + 1;
            }
            else if (position.Y + texture.Width / 2 < 0)
            {
                position.Y = boundary.Y + texture.Width / 2 - 1;
            }
        }

        public virtual bool CollisionCheck<BirdType>(GameTime gameTime)
        {
            var collidedBird = flock.Birds.Where(b => b is BirdType && b.boundingBox.Intersects(boundingBox)).FirstOrDefault();
            if (collidedBird != null)
            {
                if (collidedBird is Sparrow)
                {
                    flock.Birds.Remove((Bird)collidedBird);
                }
                ParticleEmitter explosion = new ParticleEmitter(Game1.particleTextures, ((Bird)collidedBird).position);
                Game1.particleEngineList.Add(explosion);
                return true;
            }
            else
                return false;
        }

        private void HandleEdgeCollision()
        {
            //Left and top
            float constant = 3f;
            if (position.X < border.X)
            {
                direction.X += (border.X - position.X)*constant;
            }
 
            if (position.Y < border.Y)
            {
                direction.Y += (border.Y - position.Y)*constant;
            }
 
            //Right and bottom
            Vector2 farEnd = boundary - border;
         
            if (position.X > farEnd.X)
            {
                direction.X += (farEnd.X - position.X)*constant;
            }
 
            if (position.Y > farEnd.Y)
            {
                direction.Y += (farEnd.Y - position.Y)*constant;
            }
        }

        protected void NormalizeOffset()
        {
            float offSetLength = direction.Length();

            if (offSetLength > speed)
            {
                direction = direction * speed / offSetLength;
            }
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
