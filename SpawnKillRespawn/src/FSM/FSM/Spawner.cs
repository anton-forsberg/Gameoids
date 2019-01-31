using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FSM
{
    class Spawner : GameObject
    {
        private enum SpawnDirection
        {
            Left,
            Right
        }
        private SpawnDirection spawnDirection;
        float timer, spawnTime = 60;
        public static bool spawnFSM = false, spawnFuSM = false, spawnTree = false;
        public Spawner(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            this.layerDepth = 1;
            if (position.X < Game1.window.Width / 2)
                spawnDirection = SpawnDirection.Right;
            else
                spawnDirection = SpawnDirection.Left;
                
        }

        public override void Update(GameTime gameTime)
        {
            timer++;
            
            if ((timer % spawnTime) == 0)
            {
                
                Spawn();
                if (spawnTime > 18)
                spawnTime-=0.5f;
            }
            if (timer / spawnTime > 9)
            {
                timer = 0;
                spawnTime = 60;
            }
        }
        private void Spawn()
        {
            float spawnYOffset = GameManager.tileSize * ((timer / spawnTime) - 1);
            switch (spawnDirection)
            {
                case SpawnDirection.Left:
                    if (((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 3 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 6 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 9)
                    {
                        if (spawnFuSM)
                        {
                            GameObject enemy = new FuSM(AssetsManager.enemyTex, new Vector2(position.X, position.Y + spawnYOffset), -1, this);
                            GameManager.gameObjects.Add(enemy);
                        }
                    }
                    else if (((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 2 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 5 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 8)
                    {
                        if (spawnTree)
                        {
                            GameObject enemy = new DecisionTree(AssetsManager.enemyTex, new Vector2(position.X, position.Y + spawnYOffset), -1, this);
                            GameManager.gameObjects.Add(enemy);
                        }
                    }
                    else
                    {
                        if (spawnFSM)
                        {
                            GameObject enemy = new FSM(AssetsManager.enemyTex, new Vector2(position.X, position.Y + spawnYOffset), -1, this);
                            GameManager.gameObjects.Add(enemy);
                        }
                    }
                    break;
                case SpawnDirection.Right:
                    if (((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 3 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 6 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 9)
                    {
                        if (spawnFuSM)
                        {
                            GameObject enemy = new FuSM(AssetsManager.enemyTex, new Vector2(position.X, position.Y + spawnYOffset), 1, this);
                            GameManager.gameObjects.Add(enemy);
                        }
                    }
                    else if (((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 2 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 5 || ((spawnYOffset + GameManager.tileSize) / GameManager.tileSize) == 8)
                    {
                        if (spawnTree)
                        {
                            GameObject enemy = new DecisionTree(AssetsManager.enemyTex, new Vector2(position.X, position.Y + spawnYOffset), 1, this);
                            GameManager.gameObjects.Add(enemy);
                        }
                    }
                    else
                    {
                        if (spawnFSM)
                        {
                            GameObject enemy = new FSM(AssetsManager.enemyTex, new Vector2(position.X, position.Y + spawnYOffset), 1, this);
                            GameManager.gameObjects.Add(enemy);
                        }
                    }
                    break;
            }
        }
    }
}
