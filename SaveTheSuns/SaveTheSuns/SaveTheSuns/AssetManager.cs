using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SaveTheSuns
{
    static class AssetManager
    {
        public static Texture2D smokeTex, enemyTex, playerTex, backgroundTex, darkScreen, blueShield, redShield;
        public static List<Texture2D> enemyTextures = new List<Texture2D>();
        public static List<Texture2D> sunTextures = new List<Texture2D>();
        public static List<Texture2D> rainbowTextures = new List<Texture2D>();
        public static SpriteFont font;

        public static void LoadAssets(ContentManager Content)
        {
            enemyTex = Content.Load<Texture2D>("Enemy");
            redShield = Content.Load<Texture2D>("startShieldRed");
            blueShield = Content.Load<Texture2D>("startShieldBlue");
            darkScreen = Content.Load<Texture2D>("circle");
            backgroundTex = Content.Load<Texture2D>("Background");
            smokeTex = Content.Load<Texture2D>("smoke");
            for (int i = 1; i < 13; i++)
                rainbowTextures.Add(Content.Load<Texture2D>("RainBow/rainbow" + i));
            for (int i = 0; i < 25; i++)
                enemyTextures.Add(Content.Load<Texture2D>("EnemyTextures/e" + i));
            for (int i = 1; i < 5; i++)
                sunTextures.Add(Content.Load<Texture2D>("SunTextures/sun" + i));
            font = Content.Load<SpriteFont>("Font");
            playerTex = Content.Load<Texture2D>("Player");
        }
    }
}
