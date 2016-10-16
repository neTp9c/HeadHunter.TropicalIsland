using System;
using System.Collections.Generic;
using System.Linq;

namespace HeadHunter.TropicalIsland
{
    sealed class Island
    {
        private int[,] _islandPartHeights;

        public Island(int[,] islandPartHeights)
        {
            _islandPartHeights = islandPartHeights;
        }

        public int GetVolumeStoreWaterAfterRain()
        {
            var inlandParts = GetInlandParts();
            var visitedInlandParts = new HashSet<IslandPart>();
            var totalVolumeStoreWater = 0;

            foreach (var inlandPart in inlandParts)
            {
                if(visitedInlandParts.Contains(inlandPart))
                {
                    continue;
                }
                visitedInlandParts.Add(inlandPart);

                // merge from same height inland neigbors recursive
                while (true)
                {
                    var sameHeightInlandNeighbors = new List<IslandPart>();
                    int? minHeighOfHigherInlandNeigbors = null;
                    var isThereLowerInlandNeighbor = false;
                    var isThereLowerOrEqualOutermostNeighbor = inlandPart.HasOutermostNeighbor && inlandPart.OutermostNeighborMinHeight <= inlandPart.Height;

                    foreach (var inlandNeighbor in inlandPart.InlandNeighbors)
                    {
                        if (inlandNeighbor.Height < inlandPart.Height)
                        {
                            isThereLowerInlandNeighbor = true;
                        }
                        else if(inlandNeighbor.Height > inlandPart.Height)
                        {
                            minHeighOfHigherInlandNeigbors = minHeighOfHigherInlandNeigbors.HasValue
                                ? Math.Min(minHeighOfHigherInlandNeigbors.Value, inlandNeighbor.Height)
                                : inlandNeighbor.Height;
                        }
                        else
                        {
                            sameHeightInlandNeighbors.Add(inlandNeighbor);
                        }
                    }


                    if (sameHeightInlandNeighbors.Any())
                    {
                        foreach (var sameHeightInlandNeighbor in sameHeightInlandNeighbors)
                        {
                            inlandPart.MergeFromInlandNeighbor(sameHeightInlandNeighbor);
                            visitedInlandParts.Add(sameHeightInlandNeighbor);
                        }
                    }
                    else if(isThereLowerInlandNeighbor || isThereLowerOrEqualOutermostNeighbor)
                    {
                        break;
                    }
                    else
                    {
                        // get min height of outermost and inland parts
                        int neededHeightToGoFurther = inlandPart.HasOutermostNeighbor && minHeighOfHigherInlandNeigbors.HasValue
                            ? Math.Min(inlandPart.OutermostNeighborMinHeight, minHeighOfHigherInlandNeigbors.Value)
                            : inlandPart.HasOutermostNeighbor
                                ? inlandPart.OutermostNeighborMinHeight
                                : minHeighOfHigherInlandNeigbors.Value;

                        totalVolumeStoreWater -= inlandPart.VolumeStoreWater;
                        inlandPart.AddHeight(neededHeightToGoFurther - inlandPart.Height);
                        totalVolumeStoreWater += inlandPart.VolumeStoreWater;
                    }
                }
            }

            return totalVolumeStoreWater;
        }

        private List<IslandPart> GetInlandParts()
        {
            var islandHeight = _islandPartHeights.GetLength(0);
            var islandWidth = _islandPartHeights.GetLength(1);

            var countInlandIslandParts = _islandPartHeights.Length - 2 * (islandWidth + islandHeight - 2);
            var inlandParts = new List<IslandPart>(countInlandIslandParts);

            for (var i = 1; i < islandHeight - 1; i++)
            {
                for (int j = 1; j < islandWidth - 1; j++)
                {
                    var islandPart = new IslandPart((i * islandWidth + j), _islandPartHeights[i, j], 1);

                    if(i == 1)
                    {
                        // add outermost neighbor from top
                        islandPart.AddOutermostNeighbor(_islandPartHeights[i - 1, j]);
                    }
                    else
                    {
                        // add inland neighbor from top
                        var aboveIslandPartIndex = (i - 2) * (islandWidth - 2) + (j - 1);
                        islandPart.AddInlandNeighbor(inlandParts[aboveIslandPartIndex]);
                    }
                    if(i == islandHeight - 2)
                    {
                        // add outermost neighbor from bottom
                        islandPart.AddOutermostNeighbor(_islandPartHeights[i + 1, j]);
                    }
                    
                    
                    if (j == 1)
                    {
                        // add outermost neighbor from left
                        islandPart.AddOutermostNeighbor(_islandPartHeights[i, j - 1]);
                    }
                    else
                    {
                        // add inland neighbor from left
                        var leftIslandPartIndex = (i - 1) * (islandWidth - 2) + (j - 2);
                        islandPart.AddInlandNeighbor(inlandParts[leftIslandPartIndex]);
                    }
                    if(j == islandWidth - 2)
                    {
                        // add outermost neighbor from right
                        islandPart.AddOutermostNeighbor(_islandPartHeights[i, j + 1]);
                    }

                    inlandParts.Add(islandPart);
                }
            }

            return inlandParts;
        }

        public static Island GetTropicalIslandTest1()
        {
            return new Island(new int[,] {
                { 4, 5, 4 },
                { 3, 1, 5 },
                { 5, 4, 1 }
            });
        }

        public static Island GetTropicalIslandTest2()
        {
            return new Island(new int[,] {
                { 5, 3, 4, 5 },
                { 6, 2, 1, 4 },
                { 3, 1, 1, 4 },
                { 8, 5, 4, 3 }
            });
        }
    }
}
