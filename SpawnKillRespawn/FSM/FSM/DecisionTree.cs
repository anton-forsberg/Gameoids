using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FSM
{
    class DecisionTree : GameObject
    {
        List<Bullet> bulletList = new List<Bullet>();
        int ammo = 2;
        public int health=100;
        int maxHealth=100;
        int criticalHP = 10;
        Spawner spawner;
        int shootDistance = 300;
        int reloadDistance = 400;
        float timer, reloadTime = 80;
        public float value = 50;
        string action = "Nothing!";

        public DecisionTree(Texture2D texture, Vector2 position, float speed, Spawner spawner)
        {
            this.texture = texture;
            this.position = position;
            this.color = Color.SlateGray;
            this.speed = speed;
            this.layerDepth = 0.6f;
            this.spawner = spawner;
        }

        public override void Update(GameTime gameTime)
        {
            Tree();

            #region NotRelevant
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is Spawner && g != spawner)
                {
                    if (position.X == g.boundingBox.X)
                    {
                        GameManager.player.score -= value;
                        GameManager.gameObjects.Remove(this);
                    }
                }
            }
            if (!Game1.window.Intersects(this.boundingBox))
                GameManager.gameObjects.Remove(this);

            if (boundingBox.Intersects(GameManager.player.boundingBox))
            {
                GameManager.player.health = 0;
            }
            foreach (Bullet b in bulletList.ToList())
            {
                b.Update(gameTime);
                if (!Game1.window.Intersects(b.boundingBox))
                    bulletList.Remove(b);
                if (b.boundingBox.Intersects(GameManager.player.boundingBox))
                {
                    GameManager.player.health -= 20;
                    bulletList.Remove(b);
                }

            }
            #endregion
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (Bullet b in bulletList)
            {
                b.Draw(spriteBatch);
            }
            spriteBatch.DrawString(AssetsManager.font, action, position - new Vector2(10, 16), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "A: "+ammo, position + new Vector2(0, 32), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "H: " + health, position + new Vector2(0, 42), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
        }

        //The decision tree
        public void Tree()
        {
            if (ammo > 0)
            {
                if (health <= criticalHP)
                {
                    action = "Run away!";
                    RunAway();
                }
                else if (health < maxHealth)
                {
                    if (DistanceFromPlayer < shootDistance)
                    {
                        action = "Attack!";
                        Shoot();
                    }
                    else
                    {
                        action = "Approach!";
                        MoveTowardsPlayer();
                    }
                }
                else
                {
                    action = "Roaming!";
                    position.X += speed;
                }
            }
            else
            {
                if (DistanceFromPlayer > reloadDistance)
                {
                    action = "Reload!";
                    Reload();
                }
                else
                {
                    action = "Run away!";
                    RunAway();
                }
            }
        }
        void Reload()
        {
            this.color = Color.SlateGray;
            ammo += 2;
            health -= 5;
        }
        void MoveTowardsPlayer()
        {
            this.color = Color.LawnGreen;
            Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
            direction.Normalize();
            position += direction * Math.Abs(speed*2);
        }
        void RunAway()
        {
            this.color = Color.HotPink;
            Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
            direction.Normalize();
            position -= direction * Math.Abs(speed*2);
        }
        void Shoot()
        {
            this.color = Color.Crimson;
            timer++;
            if (timer == reloadTime)
            {
                Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
                direction.Normalize();
                Bullet bullet = new Bullet(AssetsManager.bulletTex, position, direction, Color.White, 5);
                bulletList.Add(bullet);
                timer = 0;
                ammo--;
            }
        }
        float DistanceFromPlayer
        {
            get
            {
                float distance;
                Vector2.Distance(ref position, ref GameManager.player.position, out distance);
                return distance;
            }
        }
    }
}
