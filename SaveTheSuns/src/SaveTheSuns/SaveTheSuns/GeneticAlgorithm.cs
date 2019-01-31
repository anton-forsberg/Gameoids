using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveTheSuns
{
    class GeneticAlgorithm
    {
        Suns sunList;

        Population population;

        public GeneticAlgorithm()
        {
        }

        public void DoTheGA(int populationSize, int maxGenerations, int groupSize, int mutation, int chanceToUseClosesun, Suns sunList)
        {
            this.sunList = sunList;

            population = new Population();
            population.CreateRandomPopulation(populationSize, sunList, chanceToUseClosesun);

            //The starting path (after no GAing)
            FoundPath(population.BestPath, 0, false);

            int generation;
            //TODO 1 The GA Loop
            for (generation = 0; generation < maxGenerations; generation++)
                Breed(groupSize, mutation);

            //The best path the GA found
            FoundPath(population.BestPath, generation, true);
        }

        /// TODO 4 Top Breeding
        void Breed(int groupSize, int mutation)
        {
            int[] PathGroup = new int[groupSize];
            int PathCount, i, topPath, childPosition, tempPath;

            #region NotRelevant(Grouping)
            //place random Paths in the same neighborhood group
            for (PathCount = 0; PathCount < groupSize; PathCount++)
                PathGroup[PathCount] = Game1.rand.Next(population.Count);

            //sort the neighborhood group according to fitness values
            for (PathCount = 0; PathCount < groupSize - 1; PathCount++)
            {
                topPath = PathCount;
                for (i = topPath + 1; i < groupSize; i++)
                {
                    if (population[PathGroup[i]].Fitness < population[PathGroup[topPath]].Fitness)
                        topPath = i;
                }

                if (topPath != PathCount)
                {
                    tempPath = PathGroup[PathCount];
                    PathGroup[PathCount] = PathGroup[topPath];
                    PathGroup[topPath] = tempPath;
                }
            }
            #endregion

            //do crossover with the best 2 paths and replace the worst path with the created child
            childPosition = PathGroup[groupSize - 1];
            population[childPosition] = Path.Crossover(population[PathGroup[0]], population[PathGroup[1]], sunList);
            if (Game1.rand.Next(100) < mutation)
                population[childPosition].Mutate();

            population[childPosition].FitnessFunction(sunList);

            //check if the created child path has the best fitness
            if (population[childPosition].Fitness < population.BestPath.Fitness)
                population.BestPath = population[childPosition];

            //do crossover with the best 2 paths, this time in reverse order (parent1 from before is now parent2)
            childPosition = PathGroup[groupSize - 2];
            population[childPosition] = Path.Crossover(population[PathGroup[1]], population[PathGroup[0]], sunList);
            if (Game1.rand.Next(100) < mutation)
                population[childPosition].Mutate();

            population[childPosition].FitnessFunction(sunList);

            //check if the created child path has the best fitness
            if (population[childPosition].Fitness < population.BestPath.Fitness)
                population.BestPath = population[childPosition];
        }

        void FoundPath(Path bestPath, int generationNumber, bool complete)
        {
            Game1.finalPath = bestPath;
        }
    }
}
