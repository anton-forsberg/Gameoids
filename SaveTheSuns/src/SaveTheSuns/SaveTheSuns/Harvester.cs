using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SaveTheSuns
{
    class Harvester
    {
        Vector2 position;
        Texture2D texture;
        float speed = 320;
        float turningSpeed = 0.07f;
        float fixTurning = 0.002f;
        public int sunsTaken = 1;
        int targetsun;
        Vector2 movement = new Vector2(0,0);
        Path Path;
        List<Sun> sunOrder = new List<Sun>();
        bool finished;
        int texInt;
        float timer;
        Vector2 smokePosition;
        List<Trail> smokeList = new List<Trail>();

        public Harvester(Path movementPath)
        {
            MovementOrder(movementPath);
            position = sunOrder[0].Position;
            sunOrder[0].takenByEnemy = true;
            targetsun = 1;
            texture = AssetManager.enemyTextures[0];
            Path = movementPath;
        }

        public void Update(GameTime gameTime)
        {
            if (!finished && Game1.startedMoving)
                MoveTowardsSun(gameTime);
            else if (finished)
                Game1.currentState = Game1.GameState.Lost;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > 0.02f)
            {
                if (smokePosition == Vector2.Zero)
                    smokePosition = position;
                Texture2D tex = AssetManager.rainbowTextures[texInt];
                texInt++;
                if (texInt > AssetManager.rainbowTextures.Count - 1)
                    texInt = 0;
                smokeList.Add(new Trail(smokePosition, smokeList, tex));
                smokePosition = position;
                timer = 0;
            }

            foreach (Trail t in smokeList.ToList())
                t.Update();
        }

        public void MoveTowardsSun(GameTime gameTime)
        {
            if (targetsun < sunOrder.Count)
            {
                Vector2 direction = sunOrder[targetsun].Position - position;
                direction.Normalize();
                movement += direction*turningSpeed;
                turningSpeed += fixTurning;
                movement.Normalize();
                position += movement * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Vector2.Distance(position, sunOrder[targetsun].Position) < 30)
                {
                    turningSpeed = 0.05f;
                    sunOrder[targetsun].takenByEnemy = true;
                    sunsTaken++;
                    targetsun++;
                }
            }
            else
                finished = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Trail t in smokeList)
                t.Draw(spriteBatch);
            if (!Game1.startedMoving)
                spriteBatch.Draw(AssetManager.blueShield, DrawLocation(AssetManager.blueShield), Color.White);
            if (Game1.startedMoving)
                texture = AssetManager.enemyTextures[sunsTaken];
            else
                texture = AssetManager.enemyTextures[0];
            spriteBatch.Draw(texture, DrawLocation(texture), Color.White);

            if (KeyMouseReader.IsKeyDown(Keys.LeftControl))
                spriteBatch.DrawString(AssetManager.font, "D: " + Path.Fitness.ToString("0"), position+new Vector2(-30, 40), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 1f);
        }

        private Vector2 DrawLocation(Texture2D texture)
        {
                return position - new Vector2(texture.Width / 2, texture.Height / 2);
        }

        private void MovementOrder(Path movementPath)
        {
            int lastsun = 0;
            int nextsun = movementPath[0].Connection1;

            foreach (Sun sun in Game1.sunList)
            {
                sunOrder.Add(Game1.sunList[lastsun]);

                if (lastsun != movementPath[nextsun].Connection1)
                {
                    lastsun = nextsun;
                    nextsun = movementPath[nextsun].Connection1;
                }
                else
                {
                    lastsun = nextsun;
                    nextsun = movementPath[nextsun].Connection2;
                }
            }
        }
    }
}
