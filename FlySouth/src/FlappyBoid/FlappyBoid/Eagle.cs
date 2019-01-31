using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBoid
{
    class Eagle : Bird
    {
        float orientationInRadians;
        float turningSpeed = 0.07f;
        float maxSpeed = 8;
        float acceleration = 0.5f;
        public static int score = 0;
        public static int health = 3;

        public Eagle(Vector2 boundary, Flock flock, Texture2D texture)
            : base(boundary, texture)
        {
            this.flock = flock;
            speed = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
            Edges();
            if (CollisionCheck<Sparrow>(gameTime))
                score++;
            if (CollisionCheck<Raven>(gameTime))
            {
                health--;
                if (health >= 0)
                {
                    flock.Birds.Remove(this);
                    flock.Birds.Add(new Eagle(boundary, flock, texture));
                }
            }
        }




        private void Move(GameTime gameTime)
        {
            double xVector = Math.Sin(orientationInRadians);
            double yVector = Math.Cos(orientationInRadians);
            double magnitude = Math.Sqrt(xVector * xVector + yVector * yVector);
            double unitVectorX = xVector / magnitude;
            double unitVectorY = yVector / magnitude;

            if (KeyMouseReader.IsKeyDown(Keys.W))
            {
                speed += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            }
            else if (KeyMouseReader.IsKeyDown(Keys.S))
            {
                speed -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            }
            else if (speed > 0)
            {
                speed -= (acceleration/10) * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
                if (speed < 0.01f)
                    speed = 0;
            }

            if (KeyMouseReader.IsKeyDown(Keys.A))
            {
                orientationInRadians += turningSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            }
            if (KeyMouseReader.IsKeyDown(Keys.D))
            {
                orientationInRadians -= turningSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            }
            speed = MathHelper.Clamp(speed, 0, maxSpeed);
            position.X = (float)(position.X + unitVectorX * speed * gameTime.ElapsedGameTime.TotalSeconds*100);
            position.Y = (float)(position.Y + unitVectorY * speed * gameTime.ElapsedGameTime.TotalSeconds * 100);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, texture.Bounds, Color.White, -orientationInRadians+(float)(Math.PI/2), new Vector2(texture.Bounds.Width/2, texture.Bounds.Height/2), 1f, SpriteEffects.None, 0f);
        }

    }
}
