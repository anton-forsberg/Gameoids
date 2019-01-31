using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveTheSuns
{
    public class Path : List<Connection>
    {

        public Path(int size)
            : base(size)
        {
            ResetPath(size);
        }

        private double fitness;

        public double Fitness
        {
            set
            {
                fitness = value;
            }
            get
            {
                return fitness;
            }
        }

        private void ResetPath(int numberOfSuns)
        {
            this.Clear();

            Connection Connection;
            for (int i = 0; i < numberOfSuns; i++)
            {
                Connection = new Connection();
                Connection.Connection1 = -1;
                Connection.Connection2 = -1;
                this.Add(Connection);
            }
        }

        //TODO 3 Fitness Method
        public void FitnessFunction(Suns Suns)
        {
            Fitness = 0;

            int lastsun = 0;
            int nextsun = this[0].Connection1;

            foreach (Connection Connection in this)
            {
                Fitness += Suns[lastsun].Distances[nextsun];

                if (lastsun != this[nextsun].Connection1)
                {
                    lastsun = nextsun;
                    nextsun = this[nextsun].Connection1;
                }
                else
                {
                    lastsun = nextsun;
                    nextsun = this[nextsun].Connection2;
                }
            }
        }

        private static void ConnectSuns(Path Path, int[] sunUsage, int sun1, int sun2)
        {
            if (Path[sun1].Connection1 == -1)
                Path[sun1].Connection1 = sun2;
            else
                Path[sun1].Connection2 = sun2;

            if (Path[sun2].Connection1 == -1)
                Path[sun2].Connection1 = sun1;
            else
                Path[sun2].Connection2 = sun1;

            sunUsage[sun1]++;
            sunUsage[sun2]++;
        }

        private static int NextSun(Path parent, Path child, Suns sunList, int[] sunUsage, int sun)
        {
            if (TestConnection(child, sunList, sunUsage, sun, parent[sun].Connection1))
            {
                return parent[sun].Connection1;
            }
            else if (TestConnection(child, sunList, sunUsage, sun, parent[sun].Connection2))
            {
                return parent[sun].Connection2;
            }

            return -1;
        }

        private static bool TestConnection(Path Path, Suns sunList, int[] sunUsage, int sun1, int sun2)
        {
            if ((sun1 == sun2) || (sunUsage[sun1] == 2) || (sunUsage[sun2] == 2))
                return false;

            if ((sunUsage[sun1] == 0) || (sunUsage[sun2] == 0))
                return true;

            for (int direction = 0; direction < 2; direction++)
            {
                int lastsun = sun1;
                int currentsun;
                if (direction == 0)
                    currentsun = Path[sun1].Connection1;
                else
                    currentsun = Path[sun1].Connection2;
                int PathLength = 0;
                while ((currentsun != -1) && (currentsun != sun2) && (PathLength < sunList.Count - 2))
                {
                    PathLength++;
                    if (lastsun != Path[currentsun].Connection1)
                    {
                        lastsun = currentsun;
                        currentsun = Path[currentsun].Connection1;
                    }
                    else
                    {
                        lastsun = currentsun;
                        currentsun = Path[currentsun].Connection2;
                    }
                }

                if (PathLength >= sunList.Count - 2)
                    return true;

                if (currentsun == sun2)
                    return false;
            }

            return true;
        }

        /// TODO 5 Crossover
        public static Path Crossover(Path parent1, Path parent2, Suns sunList)
        {
            Path child = new Path(sunList.Count);      // the child which will be created
            int[] sunUsage = new int[sunList.Count];  // how many connections 0-2 that connect to this sun
            int sun;                                   
            int nextSun;                               // the other sun in this connection

            for (sun = 0; sun < sunList.Count; sun++)
                sunUsage[sun] = 0;

            //Keep all the connections common to both parents in the child
            for (sun = 0; sun < sunList.Count; sun++)
            {
                if (sunUsage[sun] < 2)
                {
                    if (parent1[sun].Connection1 == parent2[sun].Connection1)
                    {
                        nextSun = parent1[sun].Connection1;
                        if (TestConnection(child, sunList, sunUsage, sun, nextSun))
                        {
                            ConnectSuns(child, sunUsage, sun, nextSun);
                        }
                    }
                    if (parent1[sun].Connection2 == parent2[sun].Connection2)
                    {
                        nextSun = parent1[sun].Connection2;
                        if (TestConnection(child, sunList, sunUsage, sun, nextSun))
                        {
                            ConnectSuns(child, sunUsage, sun, nextSun);

                        }
                    }
                    if (parent1[sun].Connection1 == parent2[sun].Connection2)
                    {
                        nextSun = parent1[sun].Connection1;
                        if (TestConnection(child, sunList, sunUsage, sun, nextSun))
                        {
                            ConnectSuns(child, sunUsage, sun, nextSun);
                        }
                    }
                    if (parent1[sun].Connection2 == parent2[sun].Connection1)
                    {
                        nextSun = parent1[sun].Connection2;
                        if (TestConnection(child, sunList, sunUsage, sun, nextSun))
                        {
                            ConnectSuns(child, sunUsage, sun, nextSun);
                        }
                    }
                }
            }

            //For the connections that are not common in both parents, we switch between using connections from parent 1 and parent 2
            for (sun = 0; sun < sunList.Count; sun++)
            {
                if (sunUsage[sun] < 2)
                {
                    if (sun % 2 == 1)  // we prefer to use parent 1 on odd suns
                    {
                        nextSun = NextSun(parent1, child, sunList, sunUsage, sun);
                        if (nextSun == -1) // but if thats not possible we still go with parent 2
                            nextSun = NextSun(parent2, child, sunList, sunUsage, sun); ;
                    }
                    else // use parent 2 instead
                    {
                        nextSun = NextSun(parent2, child, sunList, sunUsage, sun);
                        if (nextSun == -1)
                            nextSun = NextSun(parent1, child, sunList, sunUsage, sun);
                    }

                    if (nextSun != -1)
                    {
                        ConnectSuns(child, sunUsage, sun, nextSun);

                        // not done yet, must have been 0 in above case.
                        if (sunUsage[sun] == 1)
                        {
                            if (sun % 2 != 1)  // use parent 1 on even suns
                            {
                                nextSun = NextSun(parent1, child, sunList, sunUsage, sun);
                                if (nextSun == -1) // use parent 2 instead
                                    nextSun = NextSun(parent2, child, sunList, sunUsage, sun);
                            }
                            else // use parent 2
                            {
                                nextSun = NextSun(parent2, child, sunList, sunUsage, sun);
                                if (nextSun == -1)
                                    nextSun = NextSun(parent1, child, sunList, sunUsage, sun);
                            }

                            if (nextSun != -1)
                                ConnectSuns(child, sunUsage, sun, nextSun);
                        }
                    }
                }
            }

            //If we would use the parents connections all the way, we would end up with disconnected loops, so instead we randomize the remaining connections
            for (sun = 0; sun < sunList.Count; sun++)
            {
                while (sunUsage[sun] < 2)
                {
                    do
                    {
                        nextSun = Game1.rand.Next(sunList.Count);  //Pick a random sun until we find one we can connect to
                    } while (!TestConnection(child, sunList, sunUsage, sun, nextSun));

                    ConnectSuns(child, sunUsage, sun, nextSun);
                }
            }

            return child;
        }

        /// TODO 6 Mutate
        public void Mutate()
        {
            int sunNumber = Game1.rand.Next(this.Count);
            Connection Connection = this[sunNumber];
            int tmpSunNumber;

            //Find the two suns which connect to the randomized sun and connect them, bypassing the randomized sun or "cutting" it out from the Path
            if (this[Connection.Connection1].Connection1 == sunNumber)
            {
                if (this[Connection.Connection2].Connection1 == sunNumber)
                {
                    tmpSunNumber = Connection.Connection2;
                    this[Connection.Connection2].Connection1 = Connection.Connection1;
                    this[Connection.Connection1].Connection1 = tmpSunNumber;
                }
                else
                {
                    tmpSunNumber = Connection.Connection2;
                    this[Connection.Connection2].Connection2 = Connection.Connection1;
                    this[Connection.Connection1].Connection1 = tmpSunNumber;
                }
            }
            else
            {
                if (this[Connection.Connection2].Connection1 == sunNumber)
                {
                    tmpSunNumber = Connection.Connection2;
                    this[Connection.Connection2].Connection1 = Connection.Connection1;
                    this[Connection.Connection1].Connection2 = tmpSunNumber;
                }
                else
                {
                    tmpSunNumber = Connection.Connection2;
                    this[Connection.Connection2].Connection2 = Connection.Connection1;
                    this[Connection.Connection1].Connection2 = tmpSunNumber;
                }

            }

            int replaceSunNumber = -1;
            do
                replaceSunNumber = Game1.rand.Next(this.Count);
            while (replaceSunNumber == sunNumber);
            Connection replaceConnection = this[replaceSunNumber];

            //Reinsert the sun which was cut from the Path
            tmpSunNumber = replaceConnection.Connection2;
            Connection.Connection2 = replaceConnection.Connection2;
            Connection.Connection1 = replaceSunNumber;
            replaceConnection.Connection2 = sunNumber;

            if (this[tmpSunNumber].Connection1 == replaceSunNumber)
                this[tmpSunNumber].Connection1 = sunNumber;
            else
                this[tmpSunNumber].Connection2 = sunNumber;
        }
    }
}
