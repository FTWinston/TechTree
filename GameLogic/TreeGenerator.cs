using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameLogic
{
    internal class TreeGenerator : IComparer<BuildingInfo>
    {
        const int absMinTreeBreadth = 1, absMaxTreeBreadth = 100, minTreeBreadth = 20, maxTreeBreadth = 38;
        const int minBuildingGroups = 4, maxBuildingGroups = 7;
        const int minDefenseBuildings = 1, maxDefenseBuildings = 3;

        public List<BuildingInfo> RootNodes { get; private set; }
        public List<BuildingInfo> AllNodes { get; private set; }

        private SortedList<BuildingInfo, int> groupsByBuilding = new SortedList<BuildingInfo, int>();
        private BuildingInfo FakeRootNode; // this is now we have a single root for our algorithms, even if we "actually" have multiple roots
        public Random r;
        private TechTree Tree;
        public List<string> UsedNames = new List<string>();
        private SortedList<BuildingInfo, int> DefaultColumns = new SortedList<BuildingInfo, int>();
        List<BuildingGroup> buildingGroups = new List<BuildingGroup>();
        private List<int> nodesByRow = new List<int>();
        private int numRows;
        public int NumUnnamed = 0;

        const int groupSeparation = 3;

        public TreeGenerator(TechTree tree, Random r) : this(tree, r, null) { }
        public TreeGenerator(TechTree tree, Random r, int? treeBreadth)
        {
            this.r = r;
            Tree = tree;

            if (!treeBreadth.HasValue || treeBreadth < absMinTreeBreadth || treeBreadth > absMaxTreeBreadth)
                treeBreadth = r.Next(minTreeBreadth - 1, maxTreeBreadth) + 1;

            int numBuildingGroups = r.Next(minBuildingGroups, maxBuildingGroups + 1);

            AllNodes = new List<BuildingInfo>();

            tree.MaxTreeRow = tree.MaxTreeColumn = 0;

            FakeRootNode = new BuildingInfo(Tree);
            FakeRootNode.TreeRow = -1;

            var group = AddGroup(FakeRootNode, true);
            buildingGroups.Add(group);
            var commandCenter = group.Buildings[0];
            RootNodes = FakeRootNode.Unlocks;

            group = AddGroup(commandCenter, false);
            buildingGroups.Add(group);

            for (int i = 2; i < numBuildingGroups; i++)
            {
                BuildingInfo parent = SelectNode(treeBreadth.Value, commandCenter);
                group = AddGroup(parent, false);
                buildingGroups.Add(group);
            }

            // each subtree should be "selected" to add a defense building into. Each subtree should have a specific point in it for defense buildings

            /*
            int numDefenseBuildings = r.Next(minDefenseBuildings, maxDefenseBuildings + 1);
            List<BuildingInfo> defenseParents = new List<BuildingInfo>();
            for (int i = 0; i < numDefenseBuildings; i++)
                defenseParents.Add(SelectNode(treeBreadth.Value, commandCenter));

            foreach ( var parent in defenseParents )
            {
                var newNode = new BuildingInfo(tree);
                newNode.Type = BuildingInfo.BuildingType.Defense;

                group = buildingGroups.Where(g => g.Buildings.Contains(parent)).First();
                group.Buildings.Add(newNode);
                group.Theme.AllocateName(newNode, this);

                parent.Unlocks.Add(newNode);
                newNode.Prerequisite = parent;
            }
            */
            var colors = HSLColor.GetDistributedSet(buildingGroups.Count, r, 140, 200);

            for ( int i=0; i<buildingGroups.Count; i++ )
            {
                group = buildingGroups[i];
                Color c = colors[i];
                foreach (var building in group.Buildings)
                    building.TreeColor = c;
            }

            foreach (var building in AllNodes)
                DefaultColumns[building] = building.TreeColumn;

            Tree.MaxTreeRow = numRows = nodesByRow.Count;
            AllNodes.Sort(this);
            PositionNodes();

            
            // shift everything left/right so that the min column is 0
            int minCol = int.MaxValue, maxCol = int.MinValue;
            foreach (var building in AllNodes)
            {
                minCol = Math.Min(minCol, building.TreeColumn);
                maxCol = Math.Max(maxCol, building.TreeColumn);
            }

            foreach (var building in AllNodes)
                building.TreeColumn -= minCol;
            
            Tree.MaxTreeColumn = maxCol - minCol;
        }

        public int Compare(BuildingInfo x, BuildingInfo y)
        {
            int row = x.TreeRow.CompareTo(y.TreeRow);
            if (row != 0)
                return row;
            return x.TreeColumn.CompareTo(y.TreeColumn);
        }

        private class BuildingGroup
        {
            public int Number;
            public TechTheme Theme;
            public BuildingInfo ParentNode;
            public BuildingInfo RootFactory;
            public List<BuildingInfo> Buildings = new List<BuildingInfo>();
            public bool Mirror;
            public int TreeColumn;
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
            1, 1, 5, 4, 1, 3, 4, 3, 1, 1, 2, 1, // 12
            2, 2, 3, 2, 1, 1, 2.5, 1, 1, 1, 1, .1, // 24
            .1, .1, .1, .1, .1, .1, 1, 1, 1, 1, // 34
        };

        private int nextGroupNumber = 0;
        private BuildingGroup AddGroup(BuildingInfo parent, bool commandSubtree)
        {
            var group = new BuildingGroup();
            group.ParentNode = parent;
            group.Number = nextGroupNumber++;
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
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    break;
                case 2:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 0);
                    break;
                case 3:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 0);
                    b3 = AddBuilding(b1, tech, group, 1);
                    break;
                case 4:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, tech, group, 0);
                    break;
                case 5:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 0);
                    b3 = AddBuilding(b2, tech, group, 0);
                    break;
                case 6:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, tech, group, 0);
                    b4 = AddBuilding(b3, tech, group, 0);
                    break;
                case 7:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, tech, group, 0);
                    b4 = AddBuilding(b1, tech, group, 1);
                    break;
                case 8:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, tech, group, 0);
                    b4 = AddBuilding(b2, tech, group, 1);
                    break;
                case 9:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 0);
                    b3 = AddBuilding(b2, tech, group, 0);
                    b4 = AddBuilding(b3, tech, group, 0);
                    break;
                case 10:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    break;
                case 11:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b1, tech, group, 1);
                    break;
                case 12:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, tech, group, 0);
                    break;
                case 13:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 1);
                    b3 = AddBuilding(b2, factory, group, 0);
                    break;
                case 14:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, tech, group, 1);
                    break;
                case 15:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b1, tech, group, 1);
                    break;
                case 16:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, -1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b4 = AddBuilding(b1, tech, group, 1);
                    break;
                case 17:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b4 = AddBuilding(b3, tech, group, 0);
                    break;
                case 18:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, tech, group, 0);
                    b4 = AddBuilding(b2, tech, group, 1);
                    break;
                case 19:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 1);
                    b3 = AddBuilding(b2, factory, group, 0);
                    b4 = AddBuilding(b2, tech, group, 1);
                    break;
                case 20:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 1);
                    b3 = AddBuilding(b2, factory, group, 0);
                    b4 = AddBuilding(b3, tech, group, 0);
                    break;
                case 21:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 1);
                    b3 = AddBuilding(b2, tech, group, 1);
                    b4 = AddBuilding(b3, factory, group, 0);
                    break;
                case 22:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, tech, group, 0);
                    b4 = AddBuilding(b2, tech, group, 1);
                    break;
                case 23:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, tech, group, 0);
                    b4 = AddBuilding(b3, tech, group, 0);
                    break;
                case 24:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b4 = AddBuilding(b3, factory, group, 0);
                    break;
                case 25:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b2, factory, group, 0);
                    b4 = AddBuilding(b3, factory, group, 0);
                    break;
                case 26:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b1, tech, group, 1);
                    b4 = AddBuilding(b2, factory, group, 0);
                    break;
                case 27:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, factory, group, 0);
                    b4 = AddBuilding(b2, tech, group, 1);
                    break;
                case 28:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 1);
                    b3 = AddBuilding(b2, factory, group, 0);
                    b4 = AddBuilding(b3, factory, group, 0);
                    break;
                case 29:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, tech, group, 1);
                    b4 = AddBuilding(b3, factory, group, 0);
                    break;
                case 30:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, 0);
                    b3 = AddBuilding(b2, factory, group, 0);
                    b4 = AddBuilding(b3, tech, group, 0);
                    break;
                case 31:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(parent, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b4 = AddBuilding(b1, factory, group, 1);
                    break;
                case 32:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, factory, group, -1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b4 = AddBuilding(b1, tech, group, 1);
                    break;
                case 33:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 0);
                    b3 = AddBuilding(b2, factory, group, -1);
                    b4 = AddBuilding(b2, factory, group, 1);
                    break;
                case 34:
                    b1 = AddBuilding(parent, factory, group, 0);
                    b2 = AddBuilding(b1, tech, group, 1);
                    b3 = AddBuilding(b1, factory, group, 0);
                    b4 = AddBuilding(b2, factory, group, 0);
                    break;
                default:
                    throw new Exception();
            }

            return group;
        }
        
        private BuildingInfo AddBuilding(BuildingInfo parent, BuildingInfo.BuildingType type, BuildingGroup group, int column = 0)
        {
            var building = new BuildingInfo(parent.Tree);
            building.Type = type;
            group.Theme.AllocateName(building, this);

            building.TreeRow = parent.TreeRow + 1;
            building.TreeColumn = column;

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
                    {
                        building.UpgradesFrom = group.RootFactory;
                        building.TreeRow = Math.Max(building.UpgradesFrom.TreeRow, parent.TreeRow) + 1;
                    }

                    building.UpgradesFrom.UpgradesTo.Add(building);
                }

            if (!parentIsUpgrade)
            {
                parent.Unlocks.Add(building);
                if (parent != FakeRootNode)
                    building.Prerequisite = parent;
            }
   
            group.Buildings.Add(building);
            groupsByBuilding.Add(building, group.Number);
            AllNodes.Add(building);

            if (nodesByRow.Count <= building.TreeRow)
            {
                while (nodesByRow.Count < building.TreeRow)
                    nodesByRow.Add(0);
                nodesByRow.Add(1);
            }

            return building;
        }

        private BuildingInfo SelectNode(int treeBreadth, BuildingInfo root = null)
        {
            BuildingInfo current = root ?? FakeRootNode;
            while (true)
                if ((current == FakeRootNode || current.Unlocks.Count > 0 || current.UpgradesTo.Count > 0) && r.Next(absMaxTreeBreadth) >= treeBreadth)
                {
                    List<BuildingInfo> list;
                    if (current.UpgradesTo.Count > 0)
                        if (current.Unlocks.Count > 0)
                            list = r.Next(2) == 0 ? current.UpgradesTo : current.Unlocks;
                        else
                            list = current.UpgradesTo;
                    else
                        list = current.Unlocks;
                    current = list[r.Next(list.Count)];
                }
                else
                    break;

            return current;
        }

        public static IEnumerable<List<T>> PermutateOrder<T>(List<T> sequence, int count)
        {
            if (count == 1)
                yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in PermutateOrder(sequence, count - 1))
                        yield return perm;

                    T tmp = sequence[count - 1];
                    sequence.RemoveAt(count - 1);
                    sequence.Insert(0, tmp);
                }
            }
        }

        public static IEnumerable<bool[]> PermutateFlags(bool[] options)
        {
            int[] flags = new int[options.Length];
            int combinations = 1;
            for (int i = 0; i < options.Length; i++)
            {
                flags[i] = combinations;
                combinations *= 2;
            }

            for (int comb = 0; comb < combinations; comb++)
            {
                for (int i = 0; i < options.Length; i++)
                    options[i] = (comb & flags[i]) != 0;

                yield return options;
            }
        }

        const int numBestStates = 5;
        private void PositionNodes()
        {
            // the list of best states are stored in order from worst to best
            List<BuildingGroup>[] bestStates = new List<BuildingGroup>[numBestStates];
            double[] bestEnergies = new double[numBestStates];
            bestStates[0] = Copy(buildingGroups);
            bestEnergies[0] = CondenseBestEnergy(bestStates[0]);
            for (int i = 1; i < numBestStates; i++)
            {
                bestStates[i] = bestStates[0];
                bestEnergies[i] = bestEnergies[0];
            }
            
            // consider every possible ordering of the groups ... but not every possible mirroring. With 7 groups, that's 5040 combinations. Only 720 for 6, 120 for 5, and 24 for 4.
            foreach (var permutation in PermutateOrder(buildingGroups, buildingGroups.Count))
            {
                double newEnergy = CondenseBestEnergy(permutation);

                int bestPos = -1;
                for ( int i=0; i<numBestStates; i++ )
                    if (newEnergy < bestEnergies[i])
                        bestPos = i;
                    else
                        break;

                if ( bestPos == -1 )
                    continue;

                for ( int i=0; i<bestPos; i++ )
                {
                    bestStates[i] = bestStates[i+1];
                    bestEnergies[i] = bestEnergies[i+1];
                }

                bestStates[bestPos] = Copy(permutation);
                bestEnergies[bestPos] = newEnergy;
            }

            // now for the best N combinations, try each combination of mirrored/unmirrored groupings

            List<BuildingGroup> bestState = bestStates[numBestStates - 1];
            double bestEnergy = bestEnergies[numBestStates - 1];

            bool[] mirrorFlags = new bool[bestState.Count];
            foreach (var permutation in PermutateFlags(mirrorFlags))
                for ( int i=numBestStates-1; i>=0; i-- )
                {
                    var state = bestStates[i];
                    for (int j = 0; j < state.Count; j++)
                        state[j].Mirror = mirrorFlags[j];

                    double newEnergy = CondenseBestEnergy(state);
                    if (newEnergy < bestEnergy)
                    {
                        bestEnergy = newEnergy;
                        bestState = Copy(state);
                    }
                }

            buildingGroups = bestState;
            PlaceBuildings(buildingGroups);
            AllNodes.Sort(this);
        }

        private List<BuildingGroup> Copy(List<BuildingGroup> state)
        {
            var output = new List<BuildingGroup>();

            foreach (var orig in state)
            {
                var copy = new BuildingGroup();
                copy.Buildings.AddRange(orig.Buildings);
                copy.ParentNode = orig.ParentNode;
                copy.Number = orig.Number;
                copy.RootFactory = orig.RootFactory;
                copy.Theme = orig.Theme;
                copy.Mirror = orig.Mirror;
                copy.TreeColumn = orig.TreeColumn;

                output.Add(copy);
            }
            
            return output;
        }

        private double Energy(List<BuildingGroup> groups)
        {
            double energy = 0;

            int minCol = int.MaxValue, maxCol = int.MinValue;
            for ( int i=0; i<AllNodes.Count; i++ )
            {
                var building = AllNodes[i];
                minCol = Math.Min(building.TreeColumn, minCol);
                maxCol = Math.Max(building.TreeColumn, maxCol);

                // if not directly below prereq, higher energy. Same with upgrades (more so), but if it has both an unlock and an upgrade, much higher energy if they're in the same column.
                int? dxPrereq = null, dyPrereq = null;
                if (building.Prerequisite != null)
                {
                    dxPrereq = building.Prerequisite.TreeColumn - building.TreeColumn;
                    dyPrereq = building.Prerequisite.TreeRow - building.TreeRow;
                    energy += dxPrereq.Value * dxPrereq.Value;
                }

                if ( building.UpgradesFrom != null )
                {
                    int dxUpgr = building.UpgradesFrom.TreeColumn - building.TreeColumn;
                    int dyUpgr = building.UpgradesFrom.TreeRow - building.TreeRow;
                    energy += dxUpgr * dxUpgr + 1; // upgrades being squint should be SLIGHTLY worse than non-upgrades being squint.

                    // if this node's upgrade and prerequisite line have the same gradient, that's very bad
                    if (dxPrereq.HasValue && (float)(dyPrereq.Value) / dxPrereq.Value == (float)(dyUpgr) / dxUpgr)
                        energy += 100;
                }

                // if any two links cross, that adds a lot of energy
                // similarly, if this node is on an "unrelated" link, that also adds a lot of energy
                foreach (var other in AllNodes)
                {
                    if (other == building)
                        continue;

                    if (other.Prerequisite != null && other.Prerequisite != building && OnLink(building, other, other.Prerequisite))
                        energy += 25;

                    if (other.UpgradesFrom != null && other.UpgradesFrom != building && OnLink(building, other, other.UpgradesFrom))
                        energy += 25;

                    if (building.Prerequisite != null)
                    {
                        if (other.Prerequisite != null)
                            if (LinksIntersect(building, building.Prerequisite, other, other.Prerequisite))
                                energy += 25;

                        if (other.UpgradesFrom != null)
                            if (LinksIntersect(building, building.Prerequisite, other, other.UpgradesFrom))
                                energy += 25;
                    }

                    if (building.UpgradesFrom != null)
                    {
                        if (other.Prerequisite != null)
                            if (LinksIntersect(building, building.UpgradesFrom, other, other.Prerequisite))
                                energy += 25;

                        if (other.UpgradesFrom != null)
                            if (LinksIntersect(building, building.UpgradesFrom, other, other.UpgradesFrom))
                                energy += 25;
                    }
                }
                
                // if more of this nodes children go to one side rather than the other, that's a bit bad
                int sumOffset = 0;
                foreach ( var child in building.Unlocks )
                {
                    var dif = child.TreeColumn - building.TreeColumn;
                    if (dif < 0)
                        sumOffset--;
                    else
                        sumOffset++;
                }

                foreach (var child in building.UpgradesTo)
                {
                    var dif = child.TreeColumn - building.TreeColumn;
                    if (dif < 0)
                        sumOffset--;
                    else
                        sumOffset++;
                }

                energy += Math.Abs(sumOffset) * 5;
            }

            energy += (maxCol - minCol) * 10;

            return energy;
        }

        private double CondenseBestEnergy(List<BuildingGroup> groups)
        {
            SpreadGroups(groups);

            var bestState = Copy(groups);
            var bestEnergy = Energy(bestState);

            foreach (var state in Condense(groups))
            {
                var energy = Energy(state);
                if (energy < bestEnergy)
                {
                    bestState = Copy(state);
                    bestEnergy = energy;
                }
            }

            groups.Clear();
            groups.AddRange(bestState);
            return bestEnergy;
        }

        private void SpreadGroups(List<BuildingGroup> groups)
        {
            int offset = groups.Count / 2;
            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                group.TreeColumn = (i-offset) * groupSeparation;
                foreach (var building in group.Buildings)
                {
                    building.TreeColumn = DefaultColumns[building];

                    if (group.Mirror)
                        building.TreeColumn = -building.TreeColumn;
                    building.TreeColumn += group.TreeColumn;
                }
            }
        }

        private void PlaceBuildings(List<BuildingGroup> groups)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                foreach (var building in group.Buildings)
                    if (group.Mirror)
                        building.TreeColumn = group.TreeColumn - DefaultColumns[building];
                    else
                        building.TreeColumn = group.TreeColumn + DefaultColumns[building];
            }
        }

        private IEnumerable<List<BuildingGroup>> Condense(List<BuildingGroup> groups)
        {
            yield return groups;

            foreach (var state in CondenseTowards(groups, g => g.ParentNode.TreeColumn))
                yield return state;
            /*
            foreach (var state in CondenseTowards(groups, g => 0))
                yield return state;*/
        }

        private IEnumerable<List<BuildingGroup>> CondenseTowards(List<BuildingGroup> groups, Func<BuildingGroup, int> towards)
        {
            // condenses this list as far as it will go. returns each step along the way
            var test = new BuildingInfo(Tree);
            bool movedAny;
            do
            {
                movedAny = false;
                AllNodes.Sort(this);

                foreach (var group in groups)
                {
                    bool canMove = true;
                    int step, dest = towards(group);
                    if (group.RootFactory.TreeColumn > dest)
                        step = -1;
                    else if (group.RootFactory.TreeColumn < dest)
                        step = 1;
                    else
                        continue;
                    foreach (var building in group.Buildings)
                    {
                        test.TreeRow = building.TreeRow;
                        test.TreeColumn = building.TreeColumn + step;
                        int pos = AllNodes.BinarySearch(test, this);
                        if (pos >= 0 && groupsByBuilding[AllNodes[pos]] != group.Number)
                        {
                            canMove = false;
                            break;
                        }
                    }

                    if (!canMove)
                        continue;

                    foreach (var building in group.Buildings)
                        building.TreeColumn += step;

                    group.TreeColumn += step;

                    movedAny = true;
                    yield return groups;
                }
            } while (movedAny);
        }

        private static bool OnLink(BuildingInfo a, BuildingInfo b1, BuildingInfo b2)
        {
            // the only links that span more than a single row are always on the same column
            if (b1.TreeColumn != b2.TreeColumn || a.TreeColumn != b1.TreeColumn)
                return false;

            if (b1.TreeRow > b2.TreeRow)
                return b1.TreeRow >= a.TreeRow && a.TreeRow >= b2.TreeRow;
            else
                return b1.TreeRow <= a.TreeRow && a.TreeRow <= b2.TreeRow;
        }

        private static bool LinksIntersect(BuildingInfo a1, BuildingInfo a2, BuildingInfo b1, BuildingInfo b2)
        {
            // if they share an endpoint, we don't count that. But if one end touches, we do.
            if (a1.TreeColumn == b1.TreeColumn && a1.TreeRow == b1.TreeRow)
                return false;
            if (a2.TreeColumn == b1.TreeColumn && a2.TreeRow == b1.TreeRow)
                return false;
            if (a2.TreeColumn == b2.TreeColumn && a2.TreeRow == b2.TreeRow)
                return false;
            if (a1.TreeColumn == b2.TreeColumn && a1.TreeRow == b2.TreeRow)
                return false;

            PointF CmP = new PointF(b1.TreeColumn - a1.TreeColumn, b1.TreeRow - a1.TreeRow);
            PointF r = new PointF(a2.TreeColumn - a1.TreeColumn, a2.TreeRow - a1.TreeRow);
            PointF s = new PointF(b2.TreeColumn - b1.TreeColumn, b2.TreeRow - b1.TreeRow);

            float CmPxr = CmP.X * r.Y - CmP.Y * r.X;
            float CmPxs = CmP.X * s.Y - CmP.Y * s.X;
            float rxs = r.X * s.Y - r.Y * s.X;

            if (CmPxr == 0f)
            {
                // Lines are collinear, and so intersect if they have any overlap. but OnLink will detect this, so we don't need to bother.
                return false/*((b1.TreeColumn - a1.TreeColumn < 0) != (b1.TreeColumn - a2.TreeColumn < 0))
                    || ((b1.TreeRow - a1.TreeRow < 0) != (b1.TreeRow - a2.TreeRow < 0))*/;
            }

            if (rxs == 0f)
                return false; // Lines are parallel.

            float rxsr = 1f / rxs;
            float t = CmPxs * rxsr;
            float u = CmPxr * rxsr;

            return (t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f);
        }
    }
}
