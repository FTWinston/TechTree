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
        public Random r;
        private TechTree Tree;
        public List<string> UsedNames = new List<string>();
        private List<int> nodesByRow = new List<int>();
        private int numRows;
        public int NumUnnamed = 0;

        public TreeGenerator(TechTree tree, Random r) : this(tree, r, null) { }
        public TreeGenerator(TechTree tree, Random r, int? treeBreadth)
        {
            this.r = r;
            Tree = tree;

            if (!treeBreadth.HasValue || treeBreadth < absMinTreeBreadth || treeBreadth > absMaxTreeBreadth)
                treeBreadth = r.Next(minTreeBreadth - 1, maxTreeBreadth) + 1;

            int numBuildingGroups = r.Next(minBuildingGroups, maxBuildingGroups + 1);
            var buildingGroups = new List<BuildingGroup>();

            AllNodes = new List<BuildingInfo>();

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
                group.Theme.AllocateName(newNode, this);

                parent.Unlocks.Add(newNode);
                newNode.Prerequisite = parent;
            }

            var colors = HSLColor.GetDistributedSet(buildingGroups.Count, r, 140, 200);

            for ( int i=0; i<buildingGroups.Count; i++ )
            {
                var group = buildingGroups[i];
                Color c = colors[i];
                foreach (var building in group.Buildings)
                    building.TreeColor = c;
            }

            numRows = nodesByRow.Count;
            AllNodes.Sort(Sort);
            PositionNodes();

            // shift everything left/right so that the min column is 0
            int minCol = int.MaxValue, maxCol = int.MinValue, maxRow = int.MinValue;
            foreach (var building in AllNodes)
            {
                minCol = Math.Min(minCol, building.TreeColumn);
                maxCol = Math.Max(maxCol, building.TreeColumn);
                maxRow = Math.Max(maxRow, building.TreeRow);
            }

            if (minCol != 0)
                foreach (var building in AllNodes)
                    building.TreeColumn -= minCol;

            Tree.MaxTreeColumn = maxCol - minCol; Tree.MaxTreeRow = maxRow;
        }

        private static int Sort(BuildingInfo n1, BuildingInfo n2)
        {
            int row = n1.TreeRow.CompareTo(n2.TreeRow);
            if (row != 0)
                return row;
            return n1.TreeColumn.CompareTo(n2.TreeColumn);
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
            1, 1, 5, 4, 1, 3, 4, 3, 1, 1, 2, 1, // 12
            2, 2, 3, 2, 1, 1, 2.5, 1, 1, 1, 1, .1, // 24
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
            group.Theme.AllocateName(building, this);

            building.TreeRow = parent.TreeRow + 1;

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
            AllNodes.Add(building);

            if (nodesByRow.Count <= building.TreeRow)
            {
                while (nodesByRow.Count < building.TreeRow)
                    nodesByRow.Add(0);
                nodesByRow.Add(1);
                building.TreeColumn = 0;
            }
            else
                building.TreeColumn = nodesByRow[building.TreeRow]++;

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
            var energy = SimulatedAnnealing();
            
            // after the annealing's done, we might be able to push things closer to their parent. As long as doing this improves the overall energy score, keep it up.
            while (ObviousShunting(ref energy))
                ;
        }

        private double SimulatedAnnealing()
        {
            List<BuildingInfo> state = Copy(AllNodes); double energy = Energy(state);
            List<BuildingInfo> bestState = state; double bestEnergy = energy;
            const int kMax = 500000, kRestartWhenStuckFor = 10000;
            int k = 0, kFound = 0, kLastMove = -1;
            while (k < kMax)
            {
                double temp = Temperature(((double)k) / kMax);
                var newState = Modify(state);
                double newEnergy = Energy(state);

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
                    state = Copy(AllNodes);
                    energy = Energy(state);
                }
                
                k++;
            }
            AllNodes = bestState;
            Console.WriteLine("Best energy {0} found on step {1} of {2}", bestEnergy, kFound, kMax);
            return bestEnergy;
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

        private List<BuildingInfo> Copy(List<BuildingInfo> state)
        {
            var lookup = new SortedList<string, BuildingInfo>();
            var output = new List<BuildingInfo>();

            foreach (var orig in state)
            {
                var copy = new BuildingInfo(orig.Tree);
                copy.Name = orig.Name;
                lookup.Add(copy.Name, copy);

                copy.TreeRow = orig.TreeRow;
                copy.TreeColumn = orig.TreeColumn;
                copy.TreeColor = orig.TreeColor;
                copy.Type = orig.Type;
                
                if ( orig.UpgradesFrom != null )
                {
                    var from = lookup[orig.UpgradesFrom.Name];
                    copy.UpgradesFrom = from;
                    from.UpgradesTo.Add(copy);
                }

                if ( orig.Prerequisite != null )
                {
                    var from = lookup[orig.Prerequisite.Name];
                    copy.Prerequisite = from;
                    from.Unlocks.Add(copy);
                }

                output.Add(copy);
            }
            
            return output;
        }

        private List<BuildingInfo> Modify(List<BuildingInfo> state)
        {
            state = Copy(state);

            switch (r.Next(5))
            {
                case 0: // shift some/all nodes in a row left/right
                    {
                        int nodePos = r.Next(state.Count);
                        int row = state[nodePos].TreeRow;

                        bool canLeft = nodePos == 0 || state[nodePos - 1].TreeRow == row;
                        bool canRight = nodePos == state.Count - 1 || state[nodePos + 1].TreeRow == row;
                        int shift = canLeft ? (canRight ? (r.Next(2) == 0 ? -1 : 1) : -1) : 1;
                        BuildingInfo node;

                        do
                        {
                            node = state[nodePos];
                            node.TreeColumn += shift;
                            nodePos += shift;
                        } while ( nodePos >= 0 && nodePos < state.Count && state[nodePos].TreeRow == row );

                        break;
                    }
                default:
                    {
                        BuildingInfo first = null, second = null;
                        int tries = 0;
                        do
                        {
                            int pos = r.Next(state.Count - 1) + 1;
                            first = state[pos];
                            second = state[pos - 1];
                            tries++;

                            if (tries > 50)
                                return state;
                        } while (first.TreeRow != second.TreeRow);

                        tries = first.TreeColumn;
                        first.TreeColumn = second.TreeColumn;
                        second.TreeColumn = tries;

                        int firstPos = state.IndexOf(first);
                        int secondPos = state.IndexOf(second);
                        state[firstPos] = second;
                        state[secondPos] = first;
                        break;
                    }
            }

            return state;
        }

        private double Energy(List<BuildingInfo> buildings)
        {
            double energy = 0;

            foreach (var building in buildings)
            {
                // if not directly below prereq, higher energy. Same with upgrades (more so), but if it has both an unlock and an upgrade, much higher energy if they're in the same column.
                int? dxPrereq = null;
                if (building.Prerequisite != null)
                {
                    dxPrereq = building.Prerequisite.TreeColumn - building.TreeColumn;
                    energy += dxPrereq.Value * dxPrereq.Value;
                }

                if ( building.UpgradesFrom != null )
                {
                    int dxUpgr = building.UpgradesFrom.TreeColumn - building.TreeColumn;
                    energy += dxUpgr * dxUpgr + 1; // upgrades being squint should be SLIGHTLY worse than non-upgrades being squint.

                    if (dxPrereq.HasValue && dxPrereq.Value == dxUpgr)
                        energy += 50;
                }

                // now if any two links cross, that adds a lot of energy
                foreach (var other in buildings)
                {
                    if (building.Prerequisite != null)
                    {
                        if (other.Prerequisite != null)
                            if (LinksCross(building, building.Prerequisite, other, other.Prerequisite))
                                energy += 40;

                        if (other.UpgradesFrom != null)
                            if (LinksCross(building, building.Prerequisite, other, other.UpgradesFrom))
                                energy += 40;
                    }

                    if (building.UpgradesFrom != null)
                    {
                        if (other.Prerequisite != null)
                            if (LinksCross(building, building.UpgradesFrom, other, other.Prerequisite))
                                energy += 40;

                        if (other.UpgradesFrom != null)
                            if (LinksCross(building, building.UpgradesFrom, other, other.UpgradesFrom))
                                energy += 40;
                    }
                }

                // and what about links that cross over the nodes themselves, but not their links?
            }

            return energy;
        }

        private bool LinksCross(BuildingInfo a1, BuildingInfo a2, BuildingInfo b1, BuildingInfo b2)
        {
            return SemiIntersect(a1, a2, b1, b2) && SemiIntersect(b1, b2, a1, a2);
        }

        private bool SemiIntersect(BuildingInfo p, BuildingInfo q, BuildingInfo e, BuildingInfo f)
        {
            var cross1 = (f.TreeColumn - e.TreeColumn) * (p.TreeRow - f.TreeRow) - (f.TreeRow - e.TreeRow) * (p.TreeColumn - f.TreeColumn);
            var cross2 = (f.TreeColumn - e.TreeColumn) * (p.TreeRow - f.TreeRow) - (f.TreeRow - e.TreeRow) * (p.TreeColumn - f.TreeColumn);
            return cross1 * cross2 < 0;
        }

        private bool ObviousShunting(ref double currentEnergy)
        {
            bool hasChanged = false;
            int? prereqOffset, upgradeOffset;
            for (int i = 0; i < AllNodes.Count; i++)
            {
                var building = AllNodes[i];
                if (building.Prerequisite != null)
                    prereqOffset = building.Prerequisite.TreeColumn - building.TreeColumn;
                else
                    prereqOffset = null;

                if (building.UpgradesFrom != null)
                    upgradeOffset = building.UpgradesFrom.TreeColumn - building.TreeColumn;
                else
                    upgradeOffset = null;

                if (prereqOffset == null)
                {
                    if (upgradeOffset == null)
                        continue;

                    hasChanged |= TryShunt(building, i, ref currentEnergy, upgradeOffset.Value);
                }
                else if (upgradeOffset == null)
                    hasChanged |= TryShunt(building, i, ref currentEnergy, prereqOffset.Value);
                else
                    hasChanged |= TryShunt(building, i, ref currentEnergy, prereqOffset.Value + upgradeOffset.Value);
            }
            Console.WriteLine("Obvious shunting got energy to {0}", currentEnergy);
            return hasChanged;
        }

        // try moving this node in the desired direction, step by step. If moving doesn't increase the overall energy, keep it.
        private bool TryShunt(BuildingInfo building, int bPos, ref double currentEnergy, int offset)
        {
            bool hasChanged = false;
            int step = offset < 0 ? -1 : 1;
            while (offset != 0)
            {
                var next = step < 0 ?
                    bPos == 0 ? null : AllNodes[bPos + step] :
                    bPos == AllNodes.Count - 1 ? null : AllNodes[bPos + step];

                if (next != null && next.TreeRow == building.TreeRow && next.TreeColumn == building.TreeColumn + step)
                    return hasChanged; // blocked

                building.TreeColumn += step;
                var energy = Energy(AllNodes);
                if (energy > currentEnergy)
                {
                    building.TreeColumn -= step; // makes it worse, undo
                    return hasChanged;
                }
                else
                {
                    currentEnergy = energy;
                    hasChanged = true;
                }

                offset -= step;
            }
            return hasChanged;
        }
    }
}
