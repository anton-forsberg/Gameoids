using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace FSM
{
    class GameManager
    {
        public static int tileSize = 32;
        public static List<GameObject> gameObjects = new List<GameObject>();
        public static List<GameObject> wallList = new List<GameObject>();
        public static Player player;
        public static Random rand;
        public static float spawnTimer;

        public GameManager()
        {
        }

        public void LoadContent(ContentManager content)
        {
            spawnTimer = 0;
            rand = new Random();
            player = new Player(AssetsManager.playerTex, new Vector2(tileSize * 19, tileSize*2));
            Spawner spawner = new Spawner(AssetsManager.spawnerTex, new Vector2(tileSize, tileSize * 12));
            Spawner spawner2 = new Spawner(AssetsManager.spawnerTex, new Vector2(tileSize * 37, tileSize * 12));
            SpawnPickups(5, Pickup.Type.health);
            SpawnPickups(5, Pickup.Type.ammo);
            gameObjects.Add(spawner);
            gameObjects.Add(spawner2);
            gameObjects.Add(player);
            
        }
        public void Reload(ContentManager content)
        {
            gameObjects.Clear();
            wallList.Clear();
            LoadContent(content);
        }
        public void Update(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer > 5)
            {
                SpawnPickups(1, Pickup.Type.health);
                SpawnPickups(1, Pickup.Type.ammo);
                spawnTimer = 0;
            }

            if (KeyMouseReader.KeyClick(Keys.D1))
            {
                Spawner.spawnFSM = !Spawner.spawnFSM;
            }
            if (KeyMouseReader.KeyClick(Keys.D3))
            {
                Spawner.spawnFuSM = !Spawner.spawnFuSM;
            }
            if (KeyMouseReader.KeyClick(Keys.D2))
            {
                Spawner.spawnTree = !Spawner.spawnTree;
            }

            foreach (GameObject g in gameObjects.ToList())
                g.Update(gameTime);

            if (wallList.Count >= 3)
                Game1.currentState = Game1.GameState.Lost;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (GameObject g in gameObjects.ToList())
                g.Draw(spriteBatch);
            foreach (GameObject g in wallList.ToList())
                g.Draw(spriteBatch);
            spriteBatch.DrawString(AssetsManager.font, "SCORE: " + player.score.ToString(), new Vector2(560, 660), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "DOOM SKULLS: " + wallList.Count.ToString() + "/3", new Vector2(956, 0), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
            if (Spawner.spawnFSM)
                spriteBatch.DrawString(AssetsManager.font, "Spawning FSM", new Vector2(400, 0), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 1f);
            if (Spawner.spawnFuSM)
                spriteBatch.DrawString(AssetsManager.font, "Spawning FuSM", new Vector2(700, 0), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 1f);
            if (Spawner.spawnTree)
                spriteBatch.DrawString(AssetsManager.font, "Spawning Tree", new Vector2(545, 0), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "H: " + player.health, new Vector2(10, 0), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "A: " + player.ammo, new Vector2(10, 32), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
            if (Game1.currentState == Game1.GameState.Lost)
            {
                spriteBatch.DrawString(AssetsManager.font, "GAME OVER", new Vector2(460, 206), Color.LimeGreen, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(AssetsManager.font, "PRESS 'R' TO PLAY AGAIN", new Vector2(360, 282), Color.LimeGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            spriteBatch.Draw(AssetsManager.background, AssetsManager.background.Bounds, AssetsManager.background.Bounds, Color.DarkSlateBlue, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void SpawnPickups(int amount, Pickup.Type spawnType)
        {
            for (int i = 0; i < amount; i++)
            {
                Pickup pickup = new Pickup(spawnType, new Vector2(tileSize * rand.Next(0, 38), tileSize * rand.Next(0, 21)));
                gameObjects.Add(pickup);
            }
        }
    }
}
