using System;
using System.Collections.Generic;
using System.Linq;

namespace HeadHunter.TropicalIsland
{
    class Program
    {
        static void Main(string[] args)
        {
            var islands = ReadIslands();
            WriteIslandsCalculation(islands);
        }

        static IEnumerable<Island> ReadIslands()
        {
            var valueDelimeter = ' ';
            var islandCount = Convert.ToInt32(Console.ReadLine());
            var islands = new List<Island>(islandCount);

            for (int islandIndex = 0; islandIndex < islandCount; islandIndex++)
            {
                var islandDimensions = Console.ReadLine().Split(valueDelimeter).Select(int.Parse).ToList();
                var islandHeight = islandDimensions[0];
                var islandWidth = islandDimensions[1];

                var islandMatrix = new int[islandHeight, islandWidth];
                for (int islandMatrixRowIndex = 0; islandMatrixRowIndex < islandHeight; islandMatrixRowIndex++)
                {
                    var islandMatrixRowHeights = Console.ReadLine().Split(valueDelimeter).Select(int.Parse);
                    var islandMatrixColumnIndex = 0;
                    foreach (var islandPartHeight in islandMatrixRowHeights)
                    {
                        islandMatrix[islandMatrixRowIndex, islandMatrixColumnIndex] = islandPartHeight;
                        islandMatrixColumnIndex++;
                    }
                }

                islands.Add(new Island(islandMatrix));
            }

            return islands;
        }

        static void WriteIslandsCalculation(IEnumerable<Island> islands)
        {
            foreach (var island in islands)
            {
                var volumeStoreWaterAfterRain = island.GetVolumeStoreWaterAfterRain();
                Console.WriteLine(volumeStoreWaterAfterRain);
            }
        }
    }
}
