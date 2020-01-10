using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModels.Definitions.Builders
{
    public partial class TreeGenerator
    {
        protected void PositionBuildings()
        {
            int mostChildren = Buildings.Values
                .Max(b => OnlyBuildings(b.Unlocks).Count());

            BuildingBuilder root = Buildings.Values.First(b => b.Prerequisite == null);
            SetRowRecursive(0, root);

            int maxRow = Buildings.Values.Max(b => b.DisplayRow);

            // First, spread everything sufficiently far apart that all children will fit even if every building has mostChildren children.
            SpreadColumnsRecursive(root, maxRow, mostChildren);

            // Contract the columns as far as possible.
            ContractColumns();
        }

        private void SetRowRecursive(int row, BuildingBuilder building)
        {
            building.DisplayRow = row;
            foreach (var child in GetBuildings(building.Unlocks))
                SetRowRecursive(row + 1, child);
        }

        private void SpreadColumnsRecursive(BuildingBuilder building, int maxRows, int maxChildren)
        {
            int childSpacing = (int)Math.Pow(maxChildren, maxRows - building.DisplayRow - 1);

            int childNum = 0;

            foreach (var child in GetBuildings(building.Unlocks))
            {
                child.DisplayColumn = building.DisplayColumn + (childNum++ * childSpacing);
                SpreadColumnsRecursive(child, maxRows, maxChildren);
            }
        }

        private void ContractColumns()
        {
            var nonRootBuildings = Buildings.Values
                .Where(b => b.Prerequisite.HasValue)
                .OrderByDescending(b => b.DisplayRow)
                .ThenBy(b => b.DisplayColumn);

            foreach (var building in nonRootBuildings)
            {
                // Shift this building (and its subtree) as far left as we can, until it is in-line with its parent, or is blocked.
                int shift = building.Prerequisite.HasValue && Buildings.TryGetValue(building.Prerequisite.Value, out var prerequisite)
                    ? DetermineMaxSubtreeLeftShift(building, building.DisplayColumn - prerequisite.DisplayColumn)
                    : building.DisplayColumn;

                if (shift > 0)
                    ShiftSubtreeLeft(building, shift);
            }

            // Now shift ALL buildings so that the left-most one is in column 0.
            var minCol = Buildings.Values.Min(b => b.DisplayColumn);
            if (minCol != 0)
            {
                foreach (var b in Buildings.Values)
                {
                    b.DisplayColumn -= minCol;
                }
            }

            // It might be possible for a building to "jump" past siblings on its left.
            foreach (var building in nonRootBuildings)
            {
                int shift = DetermineJumpLeftShift(building);

                if (shift > 0)
                {
                    ShiftSubtreeLeft(building, shift);
                }
            }
        }

        private int DetermineMaxSubtreeLeftShift(BuildingBuilder building, int maxShift)
        {
            if (maxShift == 0)
                return 0;

            // first, see how far left this building can go
            int dist = DetermineAvailableLeftShift(building, maxShift, 0);

            // then, see how far left its leftmost child can go
            BuildingBuilder leftChild = GetBuildings(building.Unlocks)
                .OrderBy(child => child.DisplayColumn)
                .FirstOrDefault();

            if (leftChild == null)
                return dist;

            if (dist == 0)
                return 0;

            // return which of the above two is the smallest
            dist = Math.Min(dist, DetermineMaxSubtreeLeftShift(leftChild, dist));

            return dist;
        }

        private int DetermineAvailableLeftShift(BuildingBuilder building, int maxShift, int dist)
        {
            do
            {
                var collision = CheckForBuilding(building.DisplayRow, building.DisplayColumn - dist - 1);
                if (collision != null)
                    break;
                dist++;
            } while (dist < maxShift);

            return dist;
        }

        private int DetermineJumpLeftShift(BuildingBuilder building)
        {
            // Find the first free space for this building, then check its descendants.
            var movers = new HashSet<BuildingBuilder> { building };
            int targetShift = 2;

            while (true)
            {
                if (CanBuildingMove(building, targetShift, movers))
                    break;
                else if (building.DisplayColumn - targetShift <= 0)
                    return 0;
                else
                    targetShift++;
            }

            // A move is invalid if the space on its right isn't a sibling.
            var onRight = CheckForBuilding(building.DisplayRow, building.DisplayColumn - targetShift + 1);
            if (onRight == null || onRight.Prerequisite != building.Prerequisite)
                return 0;

            return CanChildrenCanMove(building, targetShift, movers)
                ? targetShift
                : 0;
        }

        private bool CanBuildingMove(BuildingBuilder building, int targetShift, HashSet<BuildingBuilder> movers)
        {
            int targetColumn = building.DisplayColumn - targetShift;

            if (targetColumn < 0)
                return false;

            var existing = CheckForBuilding(building.DisplayRow, targetColumn);
            return existing == null || movers.Contains(existing);
        }

        private bool CanChildrenCanMove(BuildingBuilder building, int targetShift, HashSet<BuildingBuilder> movers)
        {
            var childBuildings = GetBuildings(building.Unlocks)
                .OrderByDescending(b => b.DisplayColumn);

            foreach (var childBuilding in childBuildings)
            {
                if (!CanBuildingMove(childBuilding, targetShift, movers))
                    return false;

                movers.Add(childBuilding);

                if (!CanChildrenCanMove(childBuilding, targetShift, movers))
                    return false;
            }

            return true;
        }

        private BuildingBuilder CheckForBuilding(int row, int column)
        {
            return Buildings.Values.SingleOrDefault(b => b.DisplayRow == row && b.DisplayColumn == column);
        }

        private void ShiftSubtreeLeft(BuildingBuilder building, int distance)
        {
            building.DisplayColumn -= distance;

            foreach (var childBuilding in GetBuildings(building.Unlocks))
                ShiftSubtreeLeft(childBuilding, distance);
        }
    }
}
