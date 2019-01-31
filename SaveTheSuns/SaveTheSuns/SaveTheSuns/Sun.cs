using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SaveTheSuns
{
    public class Sun
    {
        Texture2D texture;
        public bool takenByEnemy;
        public bool takenByPlayer;
        protected float timeBetweenFrames = 0.12f;
        protected int nrOfFrames = 1;
        protected double timeSinceLastFrame;
        private int currFrame;
        public Sun(int x, int y)
        {
            Position = new Vector2(x, y);
            texture = AssetManager.sunTextures[0];
        }

        private Vector2 position;

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        private Vector2 DrawLocation
        {
            get
            {
                return position - new Vector2((texture.Width/nrOfFrames) / 2, texture.Height / 2);
            }
        }

        private List<double> distances = new List<double>();

        public List<double> Distances
        {
            get
            {
                return distances;
            }
            set
            {
                distances = value;
            }
        }

        private List<int> closeSuns = new List<int>();

        public List<int> CloseSuns
        {
            get
            {
                return closeSuns;
            }
        }

        public void FindClosestSuns(int numberOfCloseSuns)
        {
            double shortestDistance;
            int shortestsun = 0;
            double[] dist = new double[Distances.Count];
            Distances.CopyTo(dist);

            if (numberOfCloseSuns > Distances.Count - 1)
            {
                numberOfCloseSuns = Distances.Count - 1;
            }

            closeSuns.Clear();

            for (int i = 0; i < numberOfCloseSuns; i++)
            {
                shortestDistance = Double.MaxValue;
                for (int sunNum = 0; sunNum < Distances.Count; sunNum++)
                {
                    if (dist[sunNum] < shortestDistance)
                    {
                        shortestDistance = dist[sunNum];
                        shortestsun = sunNum;
                    }
                }
                closeSuns.Add(shortestsun);
                dist[shortestsun] = Double.MaxValue;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (takenByEnemy && takenByPlayer)
            {
                nrOfFrames = 8;
                timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLastFrame >= timeBetweenFrames)
                {
                    timeSinceLastFrame -= timeBetweenFrames;
                    currFrame++;
                    if (currFrame == nrOfFrames)
                    {
                        takenByEnemy = false;
                        currFrame = 0;
                        nrOfFrames = 1;
                    }
                }
            }
            else
                nrOfFrames = 1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (takenByPlayer && takenByEnemy)
                texture = AssetManager.sunTextures[3];
            else if (takenByPlayer)
                texture = AssetManager.sunTextures[2];
            else if (takenByEnemy)
                texture = AssetManager.sunTextures[1];
            else
                texture = AssetManager.sunTextures[0];
            spriteBatch.Draw(texture, new Rectangle((int)DrawLocation.X, (int)DrawLocation.Y, texture.Width / nrOfFrames, texture.Height), new Rectangle((currFrame * texture.Width / nrOfFrames), 0, texture.Width / nrOfFrames, texture.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }
}
