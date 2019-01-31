using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FSM
{
    class Player : GameObject
    {
        List<Bullet> bulletList = new List<Bullet>();
        float timer, reloadTime = 0;
        public int health = 100;
        public int ammo = 50;
        public float score = 0;
        Vector2 movement = Vector2.Zero;
        public Player(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            this.layerDepth = 0.99f;
            speed = 5;
        }
        public override void Update(GameTime gameTime)
        {
            if (position.Y >= Game1.window.Height)
                position.Y = -GameManager.tileSize;
            if (position.Y <= -GameManager.tileSize - 1)
                position.Y = Game1.window.Height - 1;
            if (position.X <= -GameManager.tileSize)
                position.X = Game1.window.Width;
            if (position.X >= Game1.window.Width + 1)
                position.X = -GameManager.tileSize + 1;
            if (health <= 0)
                Game1.currentState = Game1.GameState.Lost;
            if (KeyMouseReader.IsKeyDown(Keys.A))
            {
                position.X -= speed;
                rotation = MathHelper.Pi;
            }
            if (KeyMouseReader.IsKeyDown(Keys.S))
            {
                position.Y += speed;
                rotation = MathHelper.Pi/2;
            }
            if (KeyMouseReader.IsKeyDown(Keys.D))
            {
                position.X += speed;
                rotation = 0f;
            }
            if (KeyMouseReader.IsKeyDown(Keys.W))
            {
                position.Y -= speed;
                rotation = MathHelper.Pi*1.5f;
            }
            position += movement;
            if (KeyMouseReader.IsKeyDown(Keys.A) && KeyMouseReader.IsKeyDown(Keys.S))
            {
                rotation = MathHelper.Pi * 0.75f;
            }
            if (KeyMouseReader.IsKeyDown(Keys.S) && KeyMouseReader.IsKeyDown(Keys.D))
            {
                rotation = MathHelper.Pi * 0.25f;
            }
            if (KeyMouseReader.IsKeyDown(Keys.D) && KeyMouseReader.IsKeyDown(Keys.W))
            {
                rotation = MathHelper.Pi * 1.75f;
            }
            if (KeyMouseReader.IsKeyDown(Keys.W) && KeyMouseReader.IsKeyDown(Keys.A))
            {
                rotation = MathHelper.Pi * 1.25f;
            }
            timer++;
            if (timer > reloadTime && KeyMouseReader.LeftClick() && ammo > 0)
            {
                Shoot();
            }
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is Pickup && boundingBox.Intersects(g.boundingBox))
                {
                    switch (((Pickup)g).thisType)
                    {
                        case Pickup.Type.ammo:
                            ammo += 20;
                            GameManager.gameObjects.Remove(g);
                            break;
                        case Pickup.Type.health:
                            health += 20;
                            GameManager.gameObjects.Remove(g);
                            break;
                        case Pickup.Type.shield:
                            GameManager.gameObjects.Remove(g);
                            break;
                    }
                }
            }

            foreach (Bullet b in bulletList.ToList())
            {
                b.Update(gameTime);
                foreach (GameObject g in GameManager.gameObjects.ToList())
                {
                    if (g is FSM && b.boundingBox.Intersects(g.boundingBox))
                    {
                        switch (((FSM)g).currentState)
                        {
                            case FSM.State.State1:
                                GameManager.gameObjects.Remove(g);
                                score += ((FSM)g).value;
                                break;
                            case FSM.State.State2:
                                ((FSM)g).currentState = FSM.State.State1;
                                score += ((FSM)g).value;
                                break;
                            case FSM.State.State3:
                                ((FSM)g).currentState = FSM.State.State2;
                                score += ((FSM)g).value;
                                break;
                        }
                        bulletList.Remove(b);
                    }
                    else if (g is FuSM && b.boundingBox.Intersects(g.boundingBox))
                    {
                        ((FuSM)g).health -= 30;
                        if (((FuSM)g).health <= 0)
                        {
                            GameManager.gameObjects.Remove(g);
                            score += ((FuSM)g).value;
                        }
                        bulletList.Remove(b);
                    }
                    else if (g is DecisionTree && b.boundingBox.Intersects(g.boundingBox))
                    {
                        ((DecisionTree)g).health -= 15;
                        if (((DecisionTree)g).health <= 0)
                        {
                            GameManager.gameObjects.Remove(g);
                            score += ((DecisionTree)g).value;
                        }
                        bulletList.Remove(b);
                    }
                }
            }
        }

        private void Shoot()
        {
            Vector2 direction = new Vector2(KeyMouseReader.MousePos().X - position.X, KeyMouseReader.MousePos().Y - position.Y);
            direction.Normalize();
            Bullet bullet = new Bullet(AssetsManager.bulletTex, position, direction, Color.Cyan, 10);
            bulletList.Add(bullet);
            timer = 0;
            ammo--;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle (boundingBox.X+16, boundingBox.Y+16, 32, 32), texture.Bounds, color, rotation, new Vector2(16, 16), SpriteEffects.None, layerDepth);
            foreach (Bullet b in bulletList)
            {
                b.Draw(spriteBatch);
            }
        }
    }
}
