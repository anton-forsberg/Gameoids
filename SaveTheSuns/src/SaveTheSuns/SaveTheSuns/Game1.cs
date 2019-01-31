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

namespace SaveTheSuns
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static bool startedMoving;
        public enum GameState
        {
            Menu,
            Select_Difficulty,
            Play,
            Won,
            Lost
        }
        bool pause;
        public static GameState currentState;
        List<Tile> background = new List<Tile>();

        public static int tileSize = 32;
        Player player;

        //GA variables
        int populationSize = 1000;
        int maxGenerations = 5;
        int mutation = 3;
        int groupSize = 5;
        int numberOfCloseSuns = 5;
        int chanceUseCloseSun = 90;

        public enum Difficulty
        {
            Very_Easy,
            Easy,
            Medium,
            Hard,
            Very_Hard
        }
        Difficulty difficulty;
        public static Random rand = new Random();
        public static Path finalPath;
        public static Suns sunList = new Suns();
        Harvester enemy;
        GeneticAlgorithm GA;
        Song sound;
        static Point screenSize;    
        public static Point windowSize
        {
            get
            {
                int y = (int)(screenSize.Y * 0.83f);
                int x = (int)(y * (float)((float)screenSize.X / (float)screenSize.Y));
                return new Point(x, y);
            }
        }
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = false;
            Window.Title = "Save The Suns!";
        }

        protected override void LoadContent()
        {
            screenSize.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenSize.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = windowSize.X;
            graphics.PreferredBackBufferHeight = windowSize.Y;
            graphics.ApplyChanges();

            AssetManager.LoadAssets(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            try
            {
                sound = Content.Load<Song>("EmptinessElectric");
                MediaPlayer.Play(sound);
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.IsRepeating = true;
            }
            catch (System.Exception ex)
            {
                Window.Title = ex.Message;
            }
            LoadGame(GameState.Menu);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            if (KeyMouseReader.KeyClick(Keys.Add))
                MediaPlayer.Volume += 0.1f;
            if (KeyMouseReader.KeyClick(Keys.Subtract))
                MediaPlayer.Volume -= 0.1f;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || KeyMouseReader.KeyClick(Keys.Escape))
                this.Exit();

            if (KeyMouseReader.KeyClick(Keys.R))
                currentState = GameState.Menu;

            if (KeyMouseReader.IsKeyDown(Keys.LeftAlt) && KeyMouseReader.KeyClick(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            switch (currentState)
            {
            case GameState.Play:
                if (KeyMouseReader.KeyClick(Keys.Space) || KeyMouseReader.KeyClick(Keys.P))
                    pause = !pause;
                if (!pause)
                {
                    enemy.Update(gameTime);
                    player.Update(gameTime);
                    foreach (Sun c in sunList)
                        c.Update(gameTime);
                }
                break;
            case GameState.Menu:
                if (KeyMouseReader.KeyClick(Keys.Enter))
                    currentState = GameState.Select_Difficulty;
                break;
            case GameState.Select_Difficulty:
                #region SoMuchCode
                if (KeyMouseReader.KeyClick(Keys.D1))
                {
                    difficulty = Difficulty.Very_Easy;
                    LoadGame(GameState.Play);
                }
                if (KeyMouseReader.KeyClick(Keys.D2))
                {
                    difficulty = Difficulty.Easy;
                    LoadGame(GameState.Play);
                }
                if (KeyMouseReader.KeyClick(Keys.D3))
                {
                    difficulty = Difficulty.Medium;
                    LoadGame(GameState.Play);
                }
                if (KeyMouseReader.KeyClick(Keys.D4))
                {
                    difficulty = Difficulty.Hard;
                    LoadGame(GameState.Play);
                }
                if (KeyMouseReader.KeyClick(Keys.D5))
                {
                    difficulty = Difficulty.Very_Hard;
                    LoadGame(GameState.Play);
                }
                break;
                #endregion
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            spriteBatch.Begin();
            foreach (Tile t in background)
                t.Draw(spriteBatch);
            foreach (Sun c in sunList)
                c.Draw(spriteBatch);
            enemy.Draw(spriteBatch);
            player.Draw(spriteBatch);

            spriteBatch.Draw(AssetManager.sunTextures[2], new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(AssetManager.font, "x " + (Suns.nrOfSuns - player.sunsSaved), new Vector2(50, 15), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.Draw(AssetManager.enemyTextures[11], new Vector2(windowSize.X-58, 3), Color.White);
            spriteBatch.DrawString(AssetManager.font, (Suns.nrOfSuns - enemy.sunsTaken) + " x", new Vector2(windowSize.X-120, 15), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);

            Vector2 endingTextPos = new Vector2(windowSize.X / 2 - 150, windowSize.Y / 2 - 50);
            Vector2 subTextPos = new Vector2(windowSize.X / 2 - 250, windowSize.Y / 2 + 50);
            Rectangle darkPos = new Rectangle(0, 0, windowSize.X+200, windowSize.Y+200);
            #region Texts
            switch (currentState)
            {
            case GameState.Lost:
                spriteBatch.Draw(AssetManager.darkScreen, darkPos, Color.Black);
                spriteBatch.DrawString(AssetManager.font, "FAILURE", endingTextPos, Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(AssetManager.font, "PRESS 'R' TO RESTART", subTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            	break;
            case GameState.Won:
                spriteBatch.Draw(AssetManager.darkScreen, darkPos, Color.Black);
                spriteBatch.DrawString(AssetManager.font, "VICTORY", endingTextPos, Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(AssetManager.font, "PRESS 'R' TO RESTART", subTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                break;
            case GameState.Select_Difficulty:
                spriteBatch.Draw(AssetManager.darkScreen, darkPos, Color.Black);
                spriteBatch.DrawString(AssetManager.font, "SELECT DIFFICULTY", endingTextPos - new Vector2(200, 0), Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(AssetManager.font, "PRESS 1-5", subTextPos+new Vector2(100, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                break;
            case GameState.Menu:
                spriteBatch.Draw(AssetManager.darkScreen, darkPos, Color.Black);
                spriteBatch.DrawString(AssetManager.font, "'ENTER' TO PLAY", endingTextPos - new Vector2(100, 0), Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);
                break;
            }
            if (pause)
            {
                spriteBatch.Draw(AssetManager.darkScreen, darkPos, Color.Black);
                spriteBatch.DrawString(AssetManager.font, "PAUSED", endingTextPos, Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(AssetManager.font, "PRESS 'SPACE' TO CONTINUE", subTextPos-new Vector2(100, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            #endregion
            spriteBatch.End();
        }

        private void LoadGame(GameState state)
        {
            sunList.Clear();
            background.Clear();
            startedMoving = false;
            #region Difficulty
            switch (difficulty)
            {
                case Difficulty.Very_Easy:
                    maxGenerations = 5;
                    break;
                case Difficulty.Easy:
                    maxGenerations = 500;
                    break;
                case Difficulty.Medium:
                    maxGenerations = 1000;
                    break;
                case Difficulty.Hard:
                    maxGenerations = 3000;
                    break;
                case Difficulty.Very_Hard:
                    maxGenerations = 10000;
                    break;
            }
            #endregion

            sunList.AddSuns();
            SolveTheTSP();
            for (int i = 0; i < (windowSize.X / tileSize) + 1; i++)
            {
                for (int j = 0; j < (windowSize.Y / tileSize) + 1; j++)
                    background.Add(new Tile(new Vector2(i * Game1.tileSize, j * Game1.tileSize)));
            }
            enemy = new Harvester(finalPath);
            player = new Player(new Vector2(rand.Next(windowSize.X), rand.Next(windowSize.Y)));
            currentState = state;
        }

        private void SolveTheTSP()
        {
            sunList.SunTotalDistances(numberOfCloseSuns);

            GA = new GeneticAlgorithm();
            GA.DoTheGA(populationSize, maxGenerations, groupSize, mutation, chanceUseCloseSun, sunList);
        }
    }
}
