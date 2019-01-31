using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FlappyBoid
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Tile> backgroundList = new List<Tile>();
        Flock flock;
        Vector2 center;
        Color textColor = Color.White;
        public static float tileSize = 32;
        public static Texture2D circle;
        Tile sun, cloud1, cloud2, cloud3;
        SpriteFont font;
        Texture2D eagleTex, meatTex, sparrowTex;
        public static List<Texture2D> particleTextures = new List<Texture2D>();
        public static List<ParticleEmitter> particleEngineList = new List<ParticleEmitter>();
        Song sound;
        public static Vector2 screenSize = new Vector2(1280, 736);
        int sparrowCount = 80;
        float playTime;
        float backgroundTimer;
        int ravenCount = 3;
        float parallaxDelay = 2;
        enum GameState
        {
            Play,
            Lost,
            Won
        }
        bool pause;
        GameState state = GameState.Play;
        float elapsed;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content" ;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            center = new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2-6);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            eagleTex = Content.Load<Texture2D>("Eagle");
            font = Content.Load<SpriteFont>("Font");
            meatTex = Content.Load<Texture2D>("Meat");
            sparrowTex = Content.Load<Texture2D>("Sparrow");
            particleTextures.Add(Content.Load<Texture2D>("Blood"));
            circle = Content.Load<Texture2D>("circle");
            try
            {
                sound = Content.Load<Song>("Vacancy");
                MediaPlayer.Play(sound);
                MediaPlayer.Volume = 0.3f;
                MediaPlayer.IsRepeating = true;
            }
            catch (System.Exception ex)
            {
                Window.Title = ex.Message;
            }
            
            LoadLevel();
            Sound();

        }

        private void LoadLevel()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    backgroundList.Add(new Tile(new Vector2(Game1.tileSize * i, Game1.tileSize * j), Content.Load<Texture2D>("Background")));
                }
            }
            backgroundList.Add(sun = new Tile(new Vector2(Game1.tileSize * 3, Game1.tileSize * 3), Content.Load<Texture2D>("Sun")));
            backgroundList.Add(cloud1 = new Tile(new Vector2(Game1.tileSize * 8, Game1.tileSize * 8), Content.Load<Texture2D>("Cloud")));
            backgroundList.Add(cloud2 = new Tile(new Vector2(Game1.tileSize * 23, Game1.tileSize * 5), Content.Load<Texture2D>("Cloud2")));
            backgroundList.Add(cloud3 = new Tile(new Vector2(Game1.tileSize * 40, Game1.tileSize * 12), Content.Load<Texture2D>("Cloud3")));
            flock = new Flock(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), sparrowCount, ravenCount, Content.Load<Texture2D>("Sparrow"), Content.Load<Texture2D>("Raven"), eagleTex);
        }
        public void Sound()
        {

        }
        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();
            if (KeyMouseReader.KeyClick(Keys.Escape))
                this.Exit();
            if (KeyMouseReader.KeyClick(Keys.P) || KeyMouseReader.KeyClick(Keys.Space))
            {
                pause = !pause;
                if (pause)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }
            if (state == GameState.Play && !pause)
            {
                foreach (ParticleEmitter p in particleEngineList.ToList())
                    p.Update(gameTime);
                backgroundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (backgroundTimer > parallaxDelay)
                {
                    sun.Update();
                    cloud1.Update();
                    cloud2.Update();
                    cloud3.Update();
                    backgroundTimer = 0;
                }
                if (elapsed > 30f)
                {
                    flock.Update(gameTime);
                    elapsed = 0f;
                }

                if (Eagle.health < 0)
                {
                    state = GameState.Lost;
                }
                if (SparrowCount() <= 0)
                    state = GameState.Won;
                base.Update(gameTime);
            }
            if (KeyMouseReader.KeyClick(Keys.R))
            {
                Restart();
            }

        }

        private void Restart()
        {
            backgroundTimer = 0;
            elapsed = 0;
            playTime = 0;
            Eagle.score = 0;
            Eagle.health = 3;
            flock.Birds.Clear();
            backgroundList.Clear();
            LoadLevel();
            state = GameState.Play;
        }
        private int SparrowCount()
        {
            int sparrowsLeft = flock.Birds.Where(b => b is Sparrow).Count();
            return sparrowsLeft;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (Tile t in backgroundList)
                t.Draw(spriteBatch);
            flock.Draw(spriteBatch);

            for (int i = 0; i < Eagle.health; i++)
            {
                spriteBatch.Draw(eagleTex, new Vector2(1110 + 50 * i, 10), Color.White);    
            }
            foreach (ParticleEmitter p in particleEngineList.ToList())
                p.Draw(spriteBatch);
            switch (state)
            {
                case GameState.Play:
                    spriteBatch.Draw(meatTex, new Vector2(10, 10), Color.White);
                    spriteBatch.DrawString(font, "x " + Eagle.score, new Vector2(50, 10), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
                    spriteBatch.Draw(sparrowTex, new Vector2(10, 694), Color.White);
                    spriteBatch.DrawString(font, "x " + SparrowCount(), new Vector2(50, 694), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
                    if (pause)
                        spriteBatch.DrawString(font, "||", new Vector2(590, 300), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                    break;
                case GameState.Lost:
                    spriteBatch.Draw(circle, new Rectangle(-500, -500, 2000, 2000), Color.Black);
                    spriteBatch.DrawString(font, "Sparrows killed per second: " + (Eagle.score / playTime).ToString("0.000"), new Vector2(center.X - 330, 0), textColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "Score: " + Eagle.score, new Vector2(center.X - 120, center.Y - 142), textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "You have died", new Vector2(center.X - 180, center.Y - 46), textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "Owl say,", new Vector2(center.X - 110, center.Y + 21), textColor, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "that wasn't very pheasant", new Vector2(center.X - 315, center.Y + 53), textColor, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "Press 'R' to restart", new Vector2(center.X - 240, center.Y + 148), textColor, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1f);
                    break;
                case GameState.Won:
                    spriteBatch.Draw(circle, new Rectangle(-500, -500, 2000, 2000), Color.Black);
                    spriteBatch.DrawString(font, "Sparrows killed per second: " + (Eagle.score/playTime).ToString("0.000"), new Vector2(center.X - 330, 0), textColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "Score: " + Eagle.score, new Vector2(center.X - 120, center.Y - 142), textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "You have won", new Vector2(center.X - 180, center.Y - 46), textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "Many birds have died", new Vector2(center.X - 250, center.Y + 21), textColor, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "but please, sparrows your pity", new Vector2(center.X - 360, center.Y + 53), textColor, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1f);
                    spriteBatch.DrawString(font, "Press 'R' to restart", new Vector2(center.X - 240, center.Y + 148), textColor, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1f);
                    break;
            }
            spriteBatch.DrawString(font, "Time: " + playTime.ToString("0.0"), new Vector2(1110, 694), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
