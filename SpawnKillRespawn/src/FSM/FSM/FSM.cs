using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FSM
{
    class FSM : GameObject
    {
        public enum State
        {
            State1,
            State2,
            State3,
            State4
        }
        public State currentState = State.State1;
        List<Bullet> bulletList = new List<Bullet>();
        Spawner spawner;
        float timer, reloadTime = 40;
        public float value = 20;
        float state1Value = 20;
        float state2Value = 50;
        float state3Value = 100;

        public FSM(Texture2D texture, Vector2 position, float speed, Spawner spawner)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            this.layerDepth = 0.6f;
            this.spawner = spawner;
        }

        public override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case State.State1:
                    State1();
                    break;
                case State.State2:
                    State2();
                    break;
                case State.State3:
                    State3();
                    break;
                case State.State4:
                    State4();
                    break;
            }

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
            spriteBatch.DrawString(AssetsManager.font, "State " + ((int)currentState).ToString(), position + new Vector2(-10, 32), Color.CornflowerBlue, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
        }

        public void State1()
        {
            this.color = Color.White;
            value = state1Value;

            //behavior
            position.X += speed;

            //check if I should transition to next state
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is FSM && g != this)
                {
                    if (((FSM)g).position == position)
                    {
                        switch (((FSM)g).currentState)
                        {
                            case State.State1:
                                ((FSM)g).currentState = State.State2;
                                currentState = State.State2;
                                break;
                            case State.State2:
                                ((FSM)g).currentState = State.State3;
                                GameManager.gameObjects.Remove(this);
                                break;
                            case State.State3:
                                ((FSM)g).currentState = State.State4;
                                GameManager.gameObjects.Remove(this);
                                break;
                        }
                    }
                }
            }
        }

        public void State2()
        {
            this.color = Color.Violet;
            value = state2Value;

            //behavior
            position.X += speed;
            timer++;
            if (timer == reloadTime)
            {
                //shoot in one direction!
                Bullet bullet = new Bullet(AssetsManager.bulletTex, position, new Vector2(0, -1), Color.Red, 5);
                bulletList.Add(bullet);
                timer = 0;
            }

            //check if I should transition to next state
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is FSM && g != this)
                {
                    if (((FSM)g).position == position)
                    {
                        switch (((FSM)g).currentState)
                        {
                            case State.State1:
                                currentState = State.State3;
                                GameManager.gameObjects.Remove(((FSM)g));
                                break;
                            case State.State2:
                                ((FSM)g).currentState = State.State3;
                                GameManager.gameObjects.Remove(this);
                                break;
                            case State.State3:
                                ((FSM)g).currentState = State.State4;
                                GameManager.gameObjects.Remove(this);
                                break;
                        }
                    }
                }
            }
        }

        public void State3()
        {
            this.color = Color.DeepSkyBlue;
            value = state3Value;

            //behavior
            position.X -= speed;
            timer++;
            if (timer == reloadTime*2)
            {
                //shoot in 4 directions!
                Vector2 vector1 = new Vector2(-1, -1);
                Vector2 vector2 = new Vector2(-1, 1);
                Vector2 vector3 = new Vector2(1, -1);
                Vector2 vector4 = new Vector2(1, 1);
                vector1.Normalize();
                vector2.Normalize();
                vector3.Normalize();
                vector4.Normalize();
                Bullet bullet1 = new Bullet(AssetsManager.bulletTex, position, vector1, Color.GreenYellow, 5);
                Bullet bullet2 = new Bullet(AssetsManager.bulletTex, position, vector2, Color.GreenYellow, 5);
                Bullet bullet3 = new Bullet(AssetsManager.bulletTex, position, vector3, Color.GreenYellow, 5);
                Bullet bullet4 = new Bullet(AssetsManager.bulletTex, position, vector4, Color.GreenYellow, 5);
                bulletList.Add(bullet1);
                bulletList.Add(bullet2);
                bulletList.Add(bullet3);
                bulletList.Add(bullet4);
                timer = 0;
            }

            //check if I should transition to next state
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is FSM && g != this)
                {
                    if (((FSM)g).position == position)
                    {
                        GameManager.gameObjects.Remove(((FSM)g));
                        GameManager.wallList.Add(this);
                        currentState = State.State4;
                        this.color = Color.CadetBlue;
                        timer = 0;
                    }
                }
            }
        }

        public void State4()
        {
            this.color = Color.CadetBlue;

            //behavior
            timer++;
            if (timer == reloadTime)
            {
                //shoot at player!
                Vector2 direction = new Vector2(GameManager.player.boundingBox.X - position.X, GameManager.player.boundingBox.Y - position.Y);
                direction.Normalize();
                Bullet bullet = new Bullet(AssetsManager.bulletTex, position, direction, Color.BlueViolet, 5);
                bulletList.Add(bullet);
                timer = 0;
            }
        }
    }
}
