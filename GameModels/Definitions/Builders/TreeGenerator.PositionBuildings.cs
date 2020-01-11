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

            var roots = Buildings.Values
                .Where(b => b.Prerequisite == null)
                .ToArray();

            foreach (var root in roots)
                SetRowRecursive(0, root);

            int maxRow = Buildings.Values.Max(b => b.DisplayRow);

            // First, spread everything sufficiently far apart that all children will fit even if every building has mostChildren children.
            SpreadColumnsRecursive(roots, maxRow, mostChildren, -1, 0);

            // Contract the columns as far as possible.
            ContractColumns();
        }

        private void SetRowRecursive(int row, BuildingBuilder building)
        {
            building.DisplayRow = row;
            foreach (var child in GetBuildings(building.Unlocks))
                SetRowRecursive(row + 1, child);
        }

        private void SpreadColumnsRecursive(IEnumerable<BuildingBuilder> buildings, int maxRows, int maxChildren, int parentRow, int parentColumn)
        {
            int spacing = (int)Math.Pow(maxChildren, maxRows - parentRow - 1);

            int childNum = 0;

            foreach (var building in buildings)
            {
                building.DisplayColumn = parentColumn + (childNum++ * spacing);

                SpreadColumnsRecursive(GetBuildings(building.Unlocks), maxRows, maxChildren, building.DisplayRow, building.DisplayColumn);
            }
        }

        private void ContractColumns()
        {
            var buildingsFromBottomLeft = Buildings.Values
                .OrderByDescending(b => b.DisplayRow)
                .ThenBy(b => b.DisplayColumn);

            foreach (var building in buildingsFromBottomLeft)
            {
                int maxShift = building.Prerequisite.HasValue && Buildings.TryGetValue(building.Prerequisite.Value, out var prerequisite)
                    ? building.DisplayColumn - prerequisite.DisplayColumn
                    : building.DisplayColumn;

                // Shift this building (and its subtree) as far left as we can, until it is in-line with its parent, or is blocked.
                int shift = DetermineMaxSubtreeLeftShift(building, maxShift);

                if (shift > 0)
                    ShiftSubtreeLeft(building, shift);
            }

            // Now shift ALL buildings so that the left-most one is in column 0.
            var minCol = Buildings.Values.Min(b => b.DisplayColumn);
            if (minCol != 0)
            {
                foreach (var b in Buildings.Values)
                    b.DisplayColumn -= minCol;
            }

            // It might be possible for a building to "jump" past siblings on its left.
            bool anyJumped;
            do
            {
                anyJumped = false;

                foreach (var building in buildingsFromBottomLeft)
                {
                    int shift = DetermineJumpLeftShift(building);

                    if (shift > 0)
                    {
                        ShiftSubtreeLeft(building, shift);
                        anyJumped = true;
                    }
                }
            } while (anyJumped);
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
            var allSiblings = Buildings.Values
                .Where(test => test.Prerequisite == building.Prerequisite)
                .ToArray();
            var minSibling = allSiblings.Min(sibling => sibling.DisplayColumn);
            var maxSibling = allSiblings.Max(sibling => sibling.DisplayColumn);

            // Find the first free space for this building, then check its descendants.
            var movers = new HashSet<BuildingBuilder> { building };
            int targetShift = 0;

            while (true)
            {
                if (CanBuildingMove(building, ++targetShift, movers))
                    break;
                else if (building.DisplayColumn - targetShift <= 0)
                    return 0;
            }

            if ((building.DisplayColumn - targetShift) < (minSibling - 1))
                return 0;

            if (building.Prerequisite.HasValue)
            {
                var prerequisite = Buildings[building.Prerequisite.Value];
                int parentCol = prerequisite.DisplayColumn;

                // Don't let the rightmost sibling end up left of the parent
                if (building.DisplayColumn == maxSibling && (building.DisplayColumn - targetShift) < parentCol)
                    return 0;
            }

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
