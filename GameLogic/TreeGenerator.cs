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

            for (int i = 1; i < numBuildingGroups; i++)
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

        private void PositionNodes()
        {
#if DEBUG
            List<double> record = new List<double>();
#endif
            List<BuildingGroup> state = Copy(buildingGroups); double energy = Energy(state);
            List<BuildingGroup> bestState = state; double bestEnergy = energy;
            const int kMax = 100000, kRestartWhenStuckFor = 1000;
            int k = 0, kFound = 0, kLastMove = -1;
            while (k < kMax)
            {
#if DEBUG
                record.Add(energy);
#endif
                double temp = Temperature(((double)k) / kMax);
                var newState = Modify(state);
                double newEnergy = Energy(newState);

                if (ShouldMove(energy, newEnergy, temp) > r.NextDouble())
                {
                    energy = newEnergy;
                    state = newState;
                    kLastMove = k;

                    if (newEnergy < bestEnergy)
                    {
                        bestEnergy = newEnergy;
                        bestState = newState;
                        kFound = k;
                    }
                }
                else if (k - kLastMove >= kRestartWhenStuckFor)
                {// restart cos we got stuck
                    Console.WriteLine("Stuck at energy {0} from step {1} to {2}, restarting...", energy, kLastMove, k);

                    kLastMove = k;
                    state = Copy(buildingGroups);
                    energy = Energy(state);
                }
                
                k++;
            }
            buildingGroups = bestState;
            Console.WriteLine("Best energy {0} found on step {1} of {2}", bestEnergy, kFound, kMax);
            Console.WriteLine("(Final energy was {0})", energy);

#if DEBUG
            Tree.Annealing = record;
            Tree.StepWhereBestFound = kFound;
#endif
        }

        double initialTemp = 30;
        private double Temperature(double fraction)
        {
            return initialTemp - fraction * initialTemp;
        }

        private double ShouldMove(double energy, double newEnergy, double temp)
        {
            if ( newEnergy < energy )
                return 1;
            var exp = Math.Exp(-(newEnergy - energy)/temp);
            return exp;
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

                output.Add(copy);
            }
            
            return output;
        }

        private List<BuildingGroup> Modify(List<BuildingGroup> state)
        {
            state = Copy(state);

            switch (r.Next(3))
            {
                case 0: // mirror the nodes of one group
                    {
                        var group = state[r.Next(state.Count)];
                        group.Mirror = !group.Mirror;
                        break;
                    }
                default: // swap the positions of two groups
                    {
                        int firstPos = r.Next(state.Count);
                        int secondPos;
                        do
                        {
                            secondPos = r.Next(state.Count);
                        } while (secondPos != firstPos);

                        var tmp = state[firstPos];
                        state[firstPos] = state[secondPos];
                        state[secondPos] = tmp;
                        break;
                    }
            }

            return state;
        }

        private double Energy(List<BuildingGroup> groups)
        {
            Condense(groups);
            double energy = 0;

            for ( int i=0; i<AllNodes.Count; i++ )
            {
                var building = AllNodes[i];
                energy += Math.Abs(building.TreeColumn);

                // if not directly below prereq, higher energy. Same with upgrades (more so), but if it has both an unlock and an upgrade, much higher energy if they're in the same column.
                int? dxPrereq = null, dyPrereq = null;
                if (building.Prerequisite != null)
                {
                    dxPrereq = building.Prerequisite.TreeColumn - building.TreeColumn;
                    dyPrereq = building.Prerequisite.TreeRow - building.TreeRow;
                    energy += dxPrereq.Value * dxPrereq.Value * 5;
                }

                if ( building.UpgradesFrom != null )
                {
                    int dxUpgr = building.UpgradesFrom.TreeColumn - building.TreeColumn;
                    int dyUpgr = building.UpgradesFrom.TreeRow - building.TreeRow;
                    energy += dxUpgr * dxUpgr * 5 + 1; // upgrades being squint should be SLIGHTLY worse than non-upgrades being squint.

                    if (dxPrereq.HasValue && (float)(dyPrereq.Value) / dxPrereq.Value == (float)(dyUpgr) / dxUpgr)
                        energy += 200;
                }

                // if any two links cross, that adds a lot of energy
                foreach (var other in AllNodes)
                {
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
            
            return energy;
        }

        private void Condense(List<BuildingGroup> groups)
        {
            int offset = groups.Count / 2;
            for ( int i=0; i<groups.Count; i++ )
            {
                var group = groups[i];
                foreach (var building in group.Buildings)
                {
                    building.TreeColumn = DefaultColumns[building];
                    
                    if (group.Mirror)
                        building.TreeColumn = 2 - building.TreeColumn;
                    building.TreeColumn += (i-offset) * groupSeparation;
                }
            }

            // now push each group center-ward as far as it will go

            // actually middleward would be nice. And not all at once, one step at a time...
            // moving through each of the groups, until nothing can move middleward any more

            // what's needed at this point is a way to get the building at a given x/y
            // will BinarySearch the old AllNodes list

            var test = new BuildingInfo(Tree);

            bool movedAny;
            do
            {
                movedAny = false;
                foreach (var group in groups)
                {
                    bool canMove = true;
                    int step;
                    if (group.RootFactory.TreeColumn > group.ParentNode.TreeColumn)
                        step = -1;
                    else if (group.RootFactory.TreeColumn < group.ParentNode.TreeColumn)
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
                    movedAny = true;
                }
            } while (movedAny);

            AllNodes.Sort(this);
        }

        private static bool LinksIntersect(BuildingInfo a1, BuildingInfo a2, BuildingInfo b1, BuildingInfo b2)
        {
            // if they just touch, we don't count that
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
                // Lines are collinear, and so intersect if they have any overlap
                return ((b1.TreeColumn - a1.TreeColumn < 0) != (b1.TreeColumn - a2.TreeColumn < 0))
                    || ((b1.TreeRow - a1.TreeRow < 0) != (b1.TreeRow - a2.TreeRow < 0));
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
