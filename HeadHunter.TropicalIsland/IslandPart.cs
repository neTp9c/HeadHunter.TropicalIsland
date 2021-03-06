﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HeadHunter.TropicalIsland
{
    sealed class IslandPart
    {
        public IslandPart(int id, int height, int square)
        {
            Id = id;
            Height = height;
            Square = square;
        }

        /// <summary>
        /// Identifier of island part, must be unique.
        /// </summary>
        public int Id { get; }
        public int Height { get; private set; }
        public int Square { get; private set; }
        public int VolumeStoreWater { get; private set; }
        public int OutermostNeighborMinHeight { get; private set; } = -1;
        public bool HasOutermostNeighbor => OutermostNeighborMinHeight > -1;

        private HashSet<IslandPart> _inlandNeighbors = new HashSet<IslandPart>();
        public IEnumerable<IslandPart> InlandNeighbors => _inlandNeighbors;

        /// <summary>
        /// Add two-way neighbor relations between this object and param object.
        /// </summary>
        public void AddInlandNeighbor(IslandPart neighbor)
        {
            _inlandNeighbors.Add(neighbor);
            neighbor._inlandNeighbors.Add(this);
        }

        /// <summary>
        /// Remove two-way neighbor relations between this object and param object.
        /// </summary>
        public void RemoveInlandNeighbor(IslandPart neighbor)
        {
            _inlandNeighbors.Remove(neighbor);
            neighbor._inlandNeighbors.Remove(this);
        }

        public void AddOutermostNeighbor(int outermostNeighborHeight)
        {
            if (outermostNeighborHeight < 0)
            {
                throw new ArgumentOutOfRangeException("outermostNeighborHeight", outermostNeighborHeight, "Must be non-negative.");
            }

            OutermostNeighborMinHeight = HasOutermostNeighbor
                ? Math.Min(OutermostNeighborMinHeight, outermostNeighborHeight)
                : outermostNeighborHeight;
        }

        /// <summary>
        /// Сombine this object with its neighbor. All inland relations from neighbor will be removed.
        /// </summary>
        public void MergeFromInlandNeighbor(IslandPart neighbor)
        {
            // raise height of result island to max from height of two island
            // calculate volume water that will be stored after raise
            if (neighbor.Height > Height)
            {
                VolumeStoreWater += (neighbor.Height - Height) * Square;
                Height = neighbor.Height;
            }
            else if (neighbor.Height < Height)
            {
                VolumeStoreWater += (Height - neighbor.Height) * neighbor.Square;
            }

            // merge squares
            Square += neighbor.Square;

            // merge outermost neighbors
            if (neighbor.HasOutermostNeighbor)
            {
                AddOutermostNeighbor(neighbor.OutermostNeighborMinHeight);
            }

            // merge inland neighbors
            foreach (var newInlandNeighbor in neighbor.InlandNeighbors)
            {
                if (newInlandNeighbor == this)
                {
                    continue;
                }

                AddInlandNeighbor(newInlandNeighbor);
            }


            while (true)
            {
                var newInlandNeighbor = neighbor.InlandNeighbors.FirstOrDefault();
                if (newInlandNeighbor == null)
                {
                    break;
                }

                // we remove all inland relations from neighbor that we merge from
                // we are tranfer its relations to new merged (combined) object
                // so objects that we merged from will have no inland relations (and other inland parts will not link to it)
                neighbor.RemoveInlandNeighbor(newInlandNeighbor);

                if (newInlandNeighbor == this)
                {
                    continue;
                }

                AddInlandNeighbor(newInlandNeighbor);
            }
        }

        public void AddHeight(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "Must be non-negative.");
            }

            if (value == 0)
            {
                return;
            }

            VolumeStoreWater += Square * value;
            Height += value;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
