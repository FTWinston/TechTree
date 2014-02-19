using System;
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

        public TreeGenerator(TechTree tree, Random r) : this(tree, r, null) { }
        public TreeGenerator(TechTree tree, Random r, int? treeBreadth)
        {
            this.r = r;
            Tree = tree;

            if (!treeBreadth.HasValue || treeBreadth < absMinTreeBreadth || treeBreadth > absMaxTreeBreadth)
                treeBreadth = r.Next(minTreeBreadth - 1, maxTreeBreadth) + 1;

            int numBuildingGroups = r.Next(minBuildingGroups, maxBuildingGroups + 1);

            tree.MaxTreeRow = tree.MaxTreeColumn = 0;
            List<string> usedNames = new List<string>();
            var themes = new SortedList<BuyableInfo, TechTheme>();

            FakeRootNode = new BuildingInfo(Tree);
            FakeRootNode.TreeRow = -1;

            var commandCenter = AddSubTree(FakeRootNode, r, usedNames, themes, true);
            RootNodes = FakeRootNode.Unlocks;

            for (int i = 1; i < numBuildingGroups; i++)
            {
                BuildingInfo parent = SelectNode(treeBreadth.Value, commandCenter);
                AddSubTree(parent, r, usedNames, themes, false);
            }

            int numDefenseBuildings = r.Next(minDefenseBuildings, maxDefenseBuildings + 1);
            List<BuildingInfo> defenseParents = new List<BuildingInfo>();
            for (int i = 0; i < numDefenseBuildings; i++)
                defenseParents.Add(SelectNode(treeBreadth.Value, commandCenter));

            foreach ( var parent in defenseParents )
            {
                var newNode = new BuildingInfo(tree);
                newNode.Type = BuildingInfo.BuildingType.Defense;

                newNode.TreeColor = parent.TreeColor;
                themes[parent].AllocateName(newNode, r, usedNames);

                parent.Unlocks.Add(newNode);
                newNode.Prerequisites.Add(parent);
            }
            
            PositionNodes();
        }

        private BuildingInfo AddSubTree(BuildingInfo parent, Random r, List<string> usedNames, SortedList<BuyableInfo, TechTheme> buildingThemes, bool commandCenterSubtree)
        {
            TechTheme theme = commandCenterSubtree ? TechTheme.Command : TechTheme.SelectRandom(r);
            Color c = RandomColor(r);

            if (commandCenterSubtree)
            {
                var commandCenter = new BuildingInfo(parent.Tree) { Type = BuildingInfo.BuildingType.Factory, TreeColor = c };
                theme.AllocateName(commandCenter, r, usedNames);
                parent.Unlocks.Add(commandCenter);

                // add a resource building - either at top level or under the command center
                var resource = new BuildingInfo(parent.Tree) { Type = BuildingInfo.BuildingType.Resource, TreeColor = c };
                theme.AllocateName(resource, r, usedNames);

                if (r.Next(3) == 0)
                {
                    resource.Prerequisites.Add(commandCenter);
                    commandCenter.Unlocks.Add(resource);
                    parent.Tree.MaxTreeRow = 1;
                }
                else
                {
                    parent.Unlocks.Add(resource);
                    parent.Tree.MaxTreeColumn = 1;
                }

                buildingThemes.Add(commandCenter, theme);
                buildingThemes.Add(resource, theme);

                return commandCenter;
            }
            else
            {
                var factory = new BuildingInfo(parent.Tree) { Type = BuildingInfo.BuildingType.Factory, TreeColor = c };
                theme.AllocateName(factory, r, usedNames);

                parent.Unlocks.Add(factory);
                factory.Prerequisites.Add(parent);


                var tech = new BuildingInfo(parent.Tree) { Type = BuildingInfo.BuildingType.Tech, TreeColor = c };
                theme.AllocateName(tech, r, usedNames);

                factory.Unlocks.Add(tech);
                tech.Prerequisites.Add(factory);


                buildingThemes.Add(factory, theme);
                buildingThemes.Add(tech, theme);

                tech = new BuildingInfo(parent.Tree) { Type = BuildingInfo.BuildingType.Tech, TreeColor = c };
                theme.AllocateName(tech, r, usedNames);

                if (r.Next(2) == 0)
                {
                    factory.Unlocks.Add(tech);
                    tech.Prerequisites.Add(factory);
                }
                else
                {
                    parent.Unlocks.Add(tech);
                    tech.Prerequisites.Add(parent);
                }

                buildingThemes.Add(tech, theme);
                return null;
            }
        }

        private Color RandomColor(Random r)
        {
            return Color.FromArgb(r.Next(128, 256), r.Next(128, 256), r.Next(128, 256));
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
