﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameLogic
{
    internal class TreeGenerator
    {
        const int absMinTreeBreadth = 1, absMaxTreeBreadth = 100, minTreeBreadth = 20, maxTreeBreadth = 38;
        const int minBuildingGroups = 4, maxBuildingGroups = 7;
        const int minDefenseBuildings = 1, maxDefenseBuildings = 3;

        public List<BuildingInfo> RootNodes { get; private set; }
        public List<BuildingInfo> AllNodes { get; private set; }

        private BuildingInfo FakeRootNode; // this is now we have a single root for our algorithms, even if we "actually" have multiple roots
        private Random r;
        private TechTree Tree;
        private List<string> usedNames = new List<string>();

        public TreeGenerator(TechTree tree, Random r) : this(tree, r, null) { }
        public TreeGenerator(TechTree tree, Random r, int? treeBreadth)
        {
            this.r = r;
            Tree = tree;

            if (!treeBreadth.HasValue || treeBreadth < absMinTreeBreadth || treeBreadth > absMaxTreeBreadth)
                treeBreadth = r.Next(minTreeBreadth - 1, maxTreeBreadth) + 1;

            int numBuildingGroups = r.Next(minBuildingGroups, maxBuildingGroups + 1);
            var buildingGroups = new List<BuildingGroup>();

            tree.MaxTreeRow = tree.MaxTreeColumn = 0;

            FakeRootNode = new BuildingInfo(Tree);
            FakeRootNode.TreeRow = -1;

            var cmdGroup = AddSubTree(FakeRootNode, true);
            buildingGroups.Add(cmdGroup);
            var commandCenter = cmdGroup.Buildings[0];
            RootNodes = FakeRootNode.Unlocks;

            for (int i = 1; i < numBuildingGroups; i++)
            {
                BuildingInfo parent = SelectNode(treeBreadth.Value, commandCenter);
                buildingGroups.Add(AddSubTree(parent, false));
            }

            int numDefenseBuildings = r.Next(minDefenseBuildings, maxDefenseBuildings + 1);
            List<BuildingInfo> defenseParents = new List<BuildingInfo>();
            for (int i = 0; i < numDefenseBuildings; i++)
                defenseParents.Add(SelectNode(treeBreadth.Value, commandCenter));

            foreach ( var parent in defenseParents )
            {
                var newNode = new BuildingInfo(tree);
                newNode.Type = BuildingInfo.BuildingType.Defense;

                var group = buildingGroups.Where(g => g.Buildings.Contains(parent)).First();
                group.Buildings.Add(newNode);
                group.Theme.AllocateName(newNode, r, usedNames);

                parent.Unlocks.Add(newNode);
                newNode.Prerequisites.Add(parent);
            }

            var colors = HSLColor.GetDistributedSet(buildingGroups.Count, r, 140, 200);

            for ( int i=0; i<buildingGroups.Count; i++ )
            {
                var group = buildingGroups[i];
                Color c = colors[i];
                foreach (var building in group.Buildings)
                    building.TreeColor = c;
            }

            PositionNodes();
        }

        private class BuildingGroup
        {
            public TechTheme Theme;
            public BuildingInfo RootFactory;
            public List<BuildingInfo> Buildings;
        }

        private double[] commandTreeWeightings = new double[] {
            3, // 1
            1, // 2
            0, 0, 0, 0, 0, 0, 0,
            3, // 10
            1, // 11
            0,
            1, // 13
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            3, // 24
            0,
            1, // 26
            0, 
            1, // 28
            0, 0,
            3, // 31
            1, // 32
            1, // 33
            1, // 34
        };

        private double[] standardTreeWeightings = new double[] {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 12
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, .1, // 24
            .1, .1, .1, .1, .1, .1, 1, 1, 1, 1, // 34
        };

        private BuildingGroup AddSubTree(BuildingInfo parent, bool commandSubtree)
        {
            var group = new BuildingGroup();
            group.Theme = commandSubtree ? TechTheme.Command : TechTheme.SelectRandom(r);
            group.Buildings = new List<BuildingInfo>();

            double[] weightings = commandSubtree ? commandTreeWeightings : standardTreeWeightings;
            double sum = 0;
            for (int i = 0; i < weightings.Length; i++)
                sum += weightings[i];

            double rand = r.NextDouble() * sum;
            sum = 0;
            int subtreeLayout = 1;
            for (int i = 0; i < weightings.Length; i++)
            {
                sum += weightings[i];
                if ( rand <= sum )
                {
                    subtreeLayout = i + 1;
                    break;
                }
            }

            var factory = BuildingInfo.BuildingType.Factory;
            var tech = commandSubtree ? BuildingInfo.BuildingType.Resource : BuildingInfo.BuildingType.Tech;

            BuildingInfo b1, b2, b3, b4;
            switch (subtreeLayout)
            {
                case 1:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);    
                    break;
                case 2:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    break;
                case 3:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b1, tech, group);
                    break;
                case 4:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, tech, group);
                    break;
                case 5:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, tech, group);
                    break;
                case 6:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, tech, group);
                    b4 = AddBuilding(b3, tech, group);
                    break;
                case 7:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, tech, group);
                    b4 = AddBuilding(b1, tech, group);
                    break;
                case 8:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, tech, group);
                    b4 = AddBuilding(b2, tech, group);
                    break;
                case 9:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, tech, group);
                    b4 = AddBuilding(b3, tech, group);
                    break;
                case 10:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    break;
                case 11:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b1, tech, group);
                    break;
                case 12:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, tech, group);
                    break;
                case 13:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, factory, group);
                    break;
                case 14:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, tech, group);
                    break;
                case 15:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b1, tech, group);
                    break;
                case 16:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b4 = AddBuilding(b1, tech, group);
                    break;
                case 17:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b4 = AddBuilding(b3, tech, group);
                    break;
                case 18:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, tech, group);
                    b4 = AddBuilding(b2, tech, group);
                    break;
                case 19:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b2, tech, group);
                    break;
                case 20:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b3, tech, group);
                    break;
                case 21:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, tech, group);
                    b4 = AddBuilding(b3, factory, group);
                    break;
                case 22:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, tech, group);
                    b4 = AddBuilding(b2, tech, group);
                    break;
                case 23:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, tech, group);
                    b4 = AddBuilding(b3, tech, group);
                    break;
                case 24:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b4 = AddBuilding(b3, factory, group);
                    break;
                case 25:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b3, factory, group);
                    break;
                case 26:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b1, tech, group);
                    b4 = AddBuilding(b2, factory, group);
                    break;
                case 27:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b2, tech, group);
                    break;
                case 28:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b3, factory, group);
                    break;
                case 29:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, tech, group);
                    b4 = AddBuilding(b3, factory, group);
                    break;
                case 30:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b3, tech, group);
                    break;
                case 31:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(parent, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b4 = AddBuilding(b1, factory, group);
                    break;
                case 32:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, factory, group);
                    b3 = AddBuilding(b1, factory, group);
                    b4 = AddBuilding(b1, tech, group);
                    break;
                case 33:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b2, factory, group);
                    b4 = AddBuilding(b2, factory, group);
                    break;
                case 34:
                    b1 = AddBuilding(parent, factory, group);
                    b2 = AddBuilding(b1, tech, group);
                    b3 = AddBuilding(b1, factory, group);
                    b4 = AddBuilding(b2, factory, group);
                    break;
                default:
                    throw new Exception();
            }

            return group;
        }
        
        private BuildingInfo AddBuilding(BuildingInfo parent, BuildingInfo.BuildingType type, BuildingGroup group)
        {
            var building = new BuildingInfo(parent.Tree);
            building.Type = type;
            group.Theme.AllocateName(building, r, usedNames);

            bool parentIsUpgrade = false;
            if (type == BuildingInfo.BuildingType.Factory)
                if (group.RootFactory == null)
                    group.RootFactory = building;
                else
                {
                    if (parent.Type == BuildingInfo.BuildingType.Factory)
                    {
                        parentIsUpgrade = true;
                        building.UpgradesFrom = parent;
                    }
                    else
                        building.UpgradesFrom = group.RootFactory;

                    building.UpgradesFrom.UpgradesTo.Add(building);
                }

            //if (!parentIsUpgrade)
            {
                parent.Unlocks.Add(building);
                if (parent != FakeRootNode)
                    building.Prerequisites.Add(parent);
            }
   
            group.Buildings.Add(building);

            return building;
        }

        private static int Sort(BuildingInfo n1, BuildingInfo n2)
        {
            int row = n1.TreeRow.CompareTo(n2.TreeRow);
            if (row != 0)
                return row;
            return n1.TreeColumn.CompareTo(n2.TreeColumn);
        }

        private BuildingInfo SelectNode(int treeBreadth, BuildingInfo root = null)
        {
            BuildingInfo current = root ?? FakeRootNode;
            while (true)
                if (current == FakeRootNode || current.Unlocks.Count > 0 && r.Next(absMaxTreeBreadth) >= treeBreadth)
                    current = current.Unlocks[r.Next(current.Unlocks.Count)];
                else
                    break;

            return current;
        }

        private void PositionNodes()
        {
            var nodesByRow = new SortedList<int, int>();
            AllNodes = new List<BuildingInfo>();
            var toProcess = new List<BuildingInfo>();
            toProcess.AddRange(RootNodes);

            while (toProcess.Count > 0)
            {
                BuildingInfo node = toProcess[0]; toProcess.RemoveAt(0);

                int row = DetermineRow(node);
                int numAtRow;
                if (!nodesByRow.TryGetValue(row, out numAtRow))
                    numAtRow = 0;

                node.TreeRow = row;
                node.TreeColumn = numAtRow;

                nodesByRow[row] = ++numAtRow;
                AllNodes.Add(node);

                foreach (BuildingInfo child in node.Unlocks)
                    if (!toProcess.Contains(child))
                        toProcess.Add(child);
            }
        }

        private static int DetermineRow(BuildingInfo node)
        {
            if (node.Prerequisites.Count == 0)
                return 0;

            int row = 0;
            foreach (BuildingInfo parent in node.Prerequisites)
                row = Math.Max(row, DetermineRow(parent));
            return row + 1;
        }

        public void SortLayout()
        {
            SortByDescendents(FakeRootNode);
            AllNodes.Sort((n1, n2) => Sort(n1, n2));
            CondenseLayout();
        }

        private int SortByDescendents(BuildingInfo node)
        {
            node.Unlocks.Sort((n1, n2) => -n1.CountDescendents().CompareTo(n2.CountDescendents()));

            int maxCol = node.TreeColumn;
            if (node.Unlocks.Count == 0)
                return maxCol;

            BuildingInfo lastChild = null;
            foreach (var child in node.Unlocks)
            {
                child.TreeColumn = maxCol;
                maxCol = SortByDescendents(child) + 1;
                lastChild = child;
            }

            maxCol--;
            node.TreeColumn = (node.TreeColumn + maxCol) / 2;
            return maxCol;
        }

        public void CondenseLayout()
        {
            bool anyMovement = false;
            while (CondenseLayout(FakeRootNode, true))
                anyMovement = true;

            if (anyMovement)
            {
                int maxCol = 0;
                foreach (var node in AllNodes)
                    maxCol = Math.Max(maxCol, node.TreeColumn);

                Tree.MaxTreeColumn = maxCol;
            }

            anyMovement = false;
            while (CondenseLayout(FakeRootNode, false))
                anyMovement = true;

            if (anyMovement)
            {
                int minCol = int.MaxValue;
                foreach (var node in AllNodes)
                    minCol = Math.Min(minCol, node.TreeColumn);

                if (minCol > 0)
                {
                    foreach (var node in AllNodes)
                        node.TreeColumn -= minCol;

                    Tree.MaxTreeColumn -= minCol;
                }
            }
        }

        private bool CondenseLayout(BuildingInfo node, bool leftward)
        {
            // if any of this node's descendents can have its entire sub-tree's column reduced by 1, then do that.
            // that will make more efficient use of space.

            bool retVal = false;
            for (int i = leftward ? 0 : node.Unlocks.Count - 1; leftward ? i < node.Unlocks.Count : i >= 0; i += leftward ? 1 : -1)
            {
                var child = node.Unlocks[i];
                if (CanShiftColumn(node, child, leftward) && (leftward ? child.TreeColumn > node.TreeColumn : child.TreeColumn < node.TreeColumn))
                {
                    ShiftColumn(child, leftward);
                    retVal = true;
                }

                if (CondenseLayout(child, leftward))
                    retVal = true;
            }
            return retVal;
        }

        private bool CanShiftColumn(BuildingInfo parent, BuildingInfo node, bool leftward)
        {
            if (leftward ? node.TreeColumn == 0 : node.TreeColumn == Tree.MaxTreeColumn)
                return false;
            int pos = AllNodes.IndexOf(node);
            BuildingInfo adjacent = AllNodes[leftward ? (pos > 0 ? pos - 1 : 0) : (pos < AllNodes.Count - 1 ? pos + 1 : 0)];
            if (adjacent.TreeRow == node.TreeRow && (leftward ? (adjacent.TreeColumn >= node.TreeColumn - 1) : (adjacent.TreeColumn <= node.TreeColumn + 1)))
                return false;

            if (node.Unlocks.Count > 0)
            {
                var endNodes = new SortedList<BuildingInfo, BuildingInfo>();
                if (leftward)
                    FindLeftmostAtEachLevel(node, endNodes);
                else
                {
                    int[] bestColumn = new int[Tree.MaxTreeRow+1];
                    FindRightmostAtEachLevel(node, endNodes, bestColumn);
                }

                // if the endmost child node at each level can shunt up, the rest are only shunting after it, anyway
                foreach (var kvp in endNodes)
                    if (!CanShiftColumn(kvp.Key, kvp.Value, leftward))
                        return false;
            }
            
            return true;
        }

        private void FindLeftmostAtEachLevel(BuildingInfo node, SortedList<BuildingInfo, BuildingInfo> endNodes)
        {// This can be simplified greatly, because the bigger trees are always leftmost, so the leftmost nodes are ALWAYS the leftmost chain
            do
            {
                var end = node.Unlocks[0];
                endNodes.Add(node, end);

                node = end;
            } while (node.Unlocks.Count > 0);
        }

        private void FindRightmostAtEachLevel(BuildingInfo node, SortedList<BuildingInfo, BuildingInfo> endNodes, int[] bestColumn)
        {
            var test = node.Unlocks[node.Unlocks.Count - 1];
            if (test.TreeColumn >= bestColumn[test.TreeRow])
            {
                endNodes[node] = test;
                bestColumn[test.TreeRow] = test.TreeColumn;
            }

            foreach (var child in node.Unlocks)
                if (child.Unlocks.Count > 0)
                    FindRightmostAtEachLevel(child, endNodes, bestColumn);
        }

        private void ShiftColumn(BuildingInfo node, bool leftward)
        {
            node.TreeColumn += leftward ? -1 : 1;
            foreach (var child in node.Unlocks)
                ShiftColumn(child, leftward);
        }
    }
}
