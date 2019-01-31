using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FSM
{
    static class AssetsManager
    {
        public static Texture2D playerTex, enemyTex, spawnerTex, bulletTex, background, health, ammo, shield, box;
        public static SpriteFont font;
        public static SoundEffectInstance instance;
        public static void LoadTextures(ContentManager content)
        {
            playerTex = content.Load<Texture2D>("Player");
            enemyTex = content.Load<Texture2D>("Enemy");
            spawnerTex = content.Load<Texture2D>("Spawner");
            bulletTex = content.Load<Texture2D>("Bullet");
            background = content.Load<Texture2D>("Background");
            health = content.Load<Texture2D>("HealthPickup");
            ammo = content.Load<Texture2D>("AmmoPickup");
            shield = content.Load<Texture2D>("ShieldPickup");
            box = content.Load<Texture2D>("Box");
            font = content.Load<SpriteFont>("Test2");
            SoundEffect bgEffect = content.Load<SoundEffect>("Idea");
            instance = bgEffect.CreateInstance();
            instance.IsLooped = true;
            instance.Play();
            instance.Volume = 0.2f;
        }
    }
}
