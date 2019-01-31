using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SaveTheSuns
{
    class Player
    {
        float orientationInRadians = (float)Math.PI;
        float turningSpeed = 0.06f;
        float maxSpeed = 4;
        float acceleration = 0.1f;
        float speed = 0;
        float timer;
        public int sunsSaved;
        Vector2 position;
        Texture2D texture;
        Vector2 smokePosition;
        List<Trail> smokeList = new List<Trail>();
        public Player(Vector2 position)
        {
            this.position = position;
            texture = AssetManager.playerTex;
        }

        public void Update(GameTime gameTime)
        {
            if (speed < 0 || speed > 0)
                Game1.startedMoving = true;
            float oldSpeed = speed;
            Move(gameTime);
            Edges();
            sunsSaved = 0;
            foreach (Sun c in Game1.sunList)
            {
                if (Vector2.Distance(position, c.Position) < 30)
                    c.takenByPlayer = true;
                if (c.takenByPlayer && !c.takenByEnemy)
                    sunsSaved++;
            }
            if (sunsSaved >= Suns.nrOfSuns)
                Game1.currentState = Game1.GameState.Won;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (oldSpeed <= 0.5f && speed > 0.5f)
                smokePosition = position;

            if (timer > 0.02 && speed > 0.5f)
            {
                if (smokePosition == Vector2.Zero)
                    smokePosition = position;
                smokeList.Add(new Trail(smokePosition, smokeList, AssetManager.smokeTex));
                smokePosition = position;
                timer = 0;
            }

            foreach (Trail t in smokeList.ToList())
                t.Update();
        }

        private void Move(GameTime gameTime)
        {
            double xVector = Math.Sin(orientationInRadians);
            double yVector = Math.Cos(orientationInRadians);
            double magnitude = Math.Sqrt(xVector * xVector + yVector * yVector);
            double unitVectorX = xVector / magnitude;
            double unitVectorY = yVector / magnitude;

            if (KeyMouseReader.IsKeyDown(Keys.W))
                speed += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            else if (KeyMouseReader.IsKeyDown(Keys.S))
                speed -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            else if (speed > 0)
            {
                speed -= (acceleration / 10) * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
                if (speed < 0.01f && speed > -0.01f)
                    speed = 0;
            }

            if (KeyMouseReader.IsKeyDown(Keys.A))
                orientationInRadians += turningSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            if (KeyMouseReader.IsKeyDown(Keys.D))
                orientationInRadians -= turningSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            speed = MathHelper.Clamp(speed, -maxSpeed/2, maxSpeed);
            position.X = (float)(position.X + unitVectorX * speed * gameTime.ElapsedGameTime.TotalSeconds * 100);
            position.Y = (float)(position.Y + unitVectorY * speed * gameTime.ElapsedGameTime.TotalSeconds * 100);
        }

        protected void Edges()
        {
            if (position.X - texture.Width / 2 > Game1.windowSize.X)
            {
                position.X = -texture.Width / 2 + 1;
            }
            else if (position.X + texture.Width / 2 < 0)
            {
                position.X = Game1.windowSize.X + texture.Width / 2 - 1;
            }
            else if (position.Y - texture.Width / 2 > Game1.windowSize.Y)
            {
                position.Y = -texture.Width / 2 + 1;
            }
            else if (position.Y + texture.Width / 2 < 0)
            {
                position.Y = Game1.windowSize.Y + texture.Width / 2 - 1;
            }
        }

        private void DrawSmoke(SpriteBatch spriteBatch)
        {
            foreach (Trail t in smokeList)
                t.Draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawSmoke(spriteBatch);
            if (!Game1.startedMoving)
                spriteBatch.Draw(AssetManager.redShield, DrawLocation(AssetManager.redShield), Color.White);
            spriteBatch.Draw(texture, position, texture.Bounds, Color.White, -orientationInRadians + (float)(Math.PI / 2), new Vector2(texture.Bounds.Width / 2, texture.Bounds.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        private Vector2 DrawLocation(Texture2D texture)
        {
            return position - new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
