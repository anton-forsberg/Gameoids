using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FSM
{
    class FuSM : GameObject
    {
        List<Bullet> bulletList = new List<Bullet>();
        Spawner spawner;
        float timer, reloadTime = 140;
        float ammo = 5;
        public float value = 100;
        public float health;
        public float maxHealth = 100;
        string priority = "Nothing!";
        float AttackPriority;
        float AmmoPickupPriority;
        float HealthPickupPriority;
        float RunAwayPriority;
        float KamikazePriority;
        int shootDistance = 400;

        public FuSM(Texture2D texture, Vector2 position, float speed, Spawner spawner)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            this.layerDepth = 0.6f;
            this.spawner = spawner;
            float speedMult = 1 - (DistanceFromPlayer / 200);
            float colorMult = MathHelper.Clamp((1 - (DistanceFromPlayer / 500)) + 0.5f, 0, 1);
            Color test = new Color(255, (int)(255 * (1 - colorMult)), (int)(255 * (colorMult / 2)));
            this.color = test;
            health = maxHealth;
        }

        public override void Update(GameTime gameTime)
        {
            FuzzyLogic();

            #region NotRelevant
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is Spawner && g != spawner)
                {
                    if (position.X == g.boundingBox.X)
                    {
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
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is Pickup && boundingBox.Intersects(g.boundingBox))
                {
                    switch (((Pickup)g).thisType)
                    {
                        case Pickup.Type.ammo:
                            ammo += 2;
                            GameManager.gameObjects.Remove(g);
                            break;
                        case Pickup.Type.health:
                            health += 10;
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
                if (!Game1.window.Intersects(b.boundingBox))
                    bulletList.Remove(b);
                if (b.boundingBox.Intersects(GameManager.player.boundingBox))
                {
                    GameManager.player.health -= 10;
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
            //spriteBatch.Draw(AssetsManager.box, new Rectangle((int)position.X, (int)position.Y + texture.Height + 5, texture.Width, 15), AssetsManager.box.Bounds, Color.DarkSlateGray, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(AssetsManager.box, new Rectangle((int)position.X+3, (int)position.Y + texture.Height + 8, (int)healthBarWidth, 9), AssetsManager.box.Bounds, Color.Cyan, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(AssetsManager.font, "FuSM", position + new Vector2(0, 32), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(AssetsManager.font, priority, position - new Vector2(10, 16), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "H: " + health.ToString("0"), position + new Vector2(0, 32), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(AssetsManager.font, "A: " + ammo.ToString("0"), position + new Vector2(0, 42), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(AssetsManager.font, "Atk: " + AttackPriority.ToString("0"), position + new Vector2(0, 52), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(AssetsManager.font, "Amm: " + AmmoPickupPriority.ToString("0"), position + new Vector2(0, 62), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(AssetsManager.font, "HP: " + HealthPickupPriority.ToString("0"), position + new Vector2(0, 72), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(AssetsManager.font, "Run: " + RunAwayPriority.ToString("0"), position + new Vector2(0, 82), Color.GreenYellow, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 1f);
        }

        public void FuzzyLogic()
        {
            //get the priority of different tasks
            AttackPriority = DistanceFromPlayer * 2;
            AmmoPickupPriority = ((ammo + 1)/3) * VectorToClosestPickup(Pickup.Type.ammo).Length();
            HealthPickupPriority = (health / 15) *VectorToClosestPickup(Pickup.Type.health).Length();
            RunAwayPriority = (DistanceFromPlayer * health) / (VectorToClosestPickup(Pickup.Type.health).Length() * 0.000000001f);
            float runDueToNoAmmo = ((ammo * 10000000000000000000) / (VectorToClosestPickup(Pickup.Type.ammo).Length())) + 200;
            if (runDueToNoAmmo < RunAwayPriority)
                RunAwayPriority = runDueToNoAmmo;
            KamikazePriority = RunAwayPriority + (DistanceFromPlayer-200);

            //try to shoot no matter what you are prioritizing
            if (DistanceFromPlayer < shootDistance && ammo > 0)
                Shoot();

            //lower priority means higher odds of happening
            if (AttackPriority < AmmoPickupPriority && AttackPriority < HealthPickupPriority && AttackPriority < RunAwayPriority && AttackPriority < KamikazePriority)
            {
                //Attack!
                MoveTowardsPlayer();
                if (DistanceFromPlayer < shootDistance && ammo > 0)
                    Shoot();
            }
            if (AmmoPickupPriority < AttackPriority && AmmoPickupPriority < HealthPickupPriority && AmmoPickupPriority < RunAwayPriority && AmmoPickupPriority < KamikazePriority)
            {
                //PickUpAmmo!
                MoveTowardsPickup(Pickup.Type.ammo);
                if (DistanceFromPlayer < shootDistance && ammo > 0)
                    Shoot();
            }
            if (HealthPickupPriority < AmmoPickupPriority && HealthPickupPriority < AttackPriority && HealthPickupPriority < RunAwayPriority && HealthPickupPriority < KamikazePriority)
            {
                //PickUpHealth!
                MoveTowardsPickup(Pickup.Type.health);
                if (DistanceFromPlayer < shootDistance && ammo > 0)
                    Shoot();
            }
            if (RunAwayPriority < AmmoPickupPriority && RunAwayPriority < HealthPickupPriority && RunAwayPriority < AttackPriority && RunAwayPriority < KamikazePriority)
            {
                //RunAway!
                RunAway();
            }
            if (KamikazePriority < AmmoPickupPriority && KamikazePriority < HealthPickupPriority && KamikazePriority < AttackPriority && KamikazePriority < RunAwayPriority)
            {
                //KAMIKAZE
                Kamikaze();
            }
        }
        void MoveTowardsPickup(Pickup.Type pickupType)
        {
            Vector2 direction = VectorToClosestPickup(pickupType);
            direction.Normalize();
            position += direction * Math.Abs(speed*2);
        }
        float DistanceFromPlayer
        {
            get
            {
                float distance = Vector2.Distance(position, GameManager.player.position);
                return distance;
            }
        }
        Vector2 VectorToClosestPickup(Pickup.Type pickupType)
        {
            float distance = 10000000000;
            float tempDistance;
            Vector2 returnVector = new Vector2(distance, distance);
            foreach (GameObject g in GameManager.gameObjects.ToList())
            {
                if (g is Pickup && ((Pickup)g).thisType == pickupType)
                {
                    Vector2 direction = new Vector2((g.boundingBox.X - position.X), (g.boundingBox.Y - position.Y));
                    tempDistance = direction.Length();
                    if (tempDistance < distance)
                    {
                        returnVector = direction;
                        distance = tempDistance;
                    }
                }
            }
            return returnVector;
        }
        void Shoot()
        {
            //Shoot faster the closer you are to the player
            float reloadMult = MathHelper.Clamp(shootDistance/DistanceFromPlayer, 1f, 5f);
            float speedMult = 1 - (DistanceFromPlayer / 200);
            Color test = new Color((int)(255 / reloadMult), (int)(100), (int)(50 * reloadMult));
            this.color = test;
            reloadTime = 140 / reloadMult;
            timer++;
            if (timer >= reloadTime)
            {
                Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
                direction.Normalize();
                Bullet bullet = new Bullet(AssetsManager.bulletTex, position, direction, Color.Yellow, 5);
                bulletList.Add(bullet);
                timer = 0;
                ammo--;
            }
        }
        void RunAway()
        {
            float speedMult = (DistanceFromPlayer / 500);
            speedMult = MathHelper.Clamp(speedMult, 0.1f, 1);
            this.color = Color.HotPink;
            Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
            direction.Normalize();
            position -= direction * Math.Abs(speed) / speedMult;
        }
        void MoveTowardsPlayer()
        {
            //Move faster the further you are from the player
            float speedMult = 1 - (DistanceFromPlayer / 200);
            float colorMult = MathHelper.Clamp((1 - (DistanceFromPlayer / 500))+0.5f, 0, 1);
            Color test = new Color(255, (int)(255 * (1-colorMult)), (int)(255 * (colorMult/2)));
            this.color = test;
            Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
            direction.Normalize();
            position += direction * Math.Abs(speed) * -speedMult;
        }
        void Kamikaze()
        {
            this.color = Color.Red;
            Vector2 direction = new Vector2((GameManager.player.boundingBox.X - position.X), (GameManager.player.boundingBox.Y - position.Y));
            direction.Normalize();
            position += direction * Math.Abs(speed*2);
        }
        void Reload()
        {
            ammo += 5;
            health -= 5;
        }
    }
}
