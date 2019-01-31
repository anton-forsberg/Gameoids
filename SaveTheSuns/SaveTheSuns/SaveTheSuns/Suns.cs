using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace SaveTheSuns
{
    public class Suns : List<Sun>
    {
        public static int nrOfSuns = 24;

        public void SunTotalDistances(int numberOfCloseSuns)
        {
            foreach (Sun sun in this)
            {
                sun.Distances.Clear();

                for (int i = 0; i < Count; i++)
                    sun.Distances.Add(Vector2.Distance(this[i].Position, sun.Position));
            }

            foreach (Sun sun in this)
            {
                sun.FindClosestSuns(numberOfCloseSuns);
            }
        }

        public void AddSuns()
        {
            for (int i = 0; i < nrOfSuns; i++)
                this.Add(new Sun(Game1.rand.Next(0, (Game1.windowSize.X / Game1.tileSize)) * Game1.tileSize + 16, Game1.rand.Next(2, (Game1.windowSize.Y / Game1.tileSize)) * Game1.tileSize + 16));
        }
    }
}
