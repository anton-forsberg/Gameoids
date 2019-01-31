using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveTheSuns
{
    class Population : List<Path>
    {
        private Path bestPath = null;

        public Path BestPath
        {
            set
            {
                bestPath = value;
            }
            get
            {
                return bestPath;
            }
        }

        //TODO 2 Initializes the Population
        public void CreateRandomPopulation(int populationSize, Suns sunList, int chanceToUseClosesun)
        {
            int firstsun, lastsun, nextsun;

            for (int PathCount = 0; PathCount < populationSize; PathCount++)
            {
                Path Path = new Path(sunList.Count);

                // Create a starting point for this Path
                firstsun = Game1.rand.Next(sunList.Count);
                lastsun = firstsun;

                for (int sun = 0; sun < sunList.Count - 1; sun++)
                {
                    do
                    {
                        // Keep picking random Suns for the next sun, until we find one we haven't been to.
                        if ((Game1.rand.Next(100) < chanceToUseClosesun) && (sunList[sun].CloseSuns.Count > 0))
                        {
                            // 75% chance will will pick a sun that is close to this one
                            nextsun = sunList[sun].CloseSuns[Game1.rand.Next(sunList[sun].CloseSuns.Count)];
                        }
                        else
                        {
                            // Otherwise, pick a completely random sun.
                            nextsun = Game1.rand.Next(sunList.Count);
                        }
                        // Make sure we haven't been here, and make sure it isn't where we are at now.
                    } while ((Path[nextsun].Connection2 != -1) || (nextsun == lastsun));

                    // When going from sun A to B, [1] on A = B and [1] on sun B = A
                    Path[lastsun].Connection2 = nextsun;
                    Path[nextsun].Connection1 = lastsun;
                    lastsun = nextsun;
                }

                // Connect the last 2 Suns.
                Path[lastsun].Connection2 = firstsun;
                Path[firstsun].Connection1 = lastsun;

                Path.FitnessFunction(sunList);
                
                Add(Path);

                if ((bestPath == null) || (Path.Fitness < bestPath.Fitness))
                {
                    BestPath = Path;
                }
            }
        }
    }
}
