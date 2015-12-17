using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Generation
{
    public class TreeGenerator
    {
        public static TechTree Generate(Complexity complexity = Complexity.Normal, int? seed = null)
        {
            if (seed == null)
                seed = new Random().Next(int.MinValue, int.MaxValue);
            
            var generator = new TreeGenerator()
            {
                Seed = seed.Value,
                TreeComplexity = complexity
            };
            return generator.Generate();
        }

        private TreeGenerator() { }
        internal int Seed { get; private set; }
        internal Random Random { get; private set; }
        internal TechTree Tree { get; private set; }
        internal Complexity TreeComplexity { get; private set; }

        const string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZΔΣΦΨΩαβγδεζηθλμξπςφω?#@£$%&☺♀♂♠♣♥♦";
        int nextSymbolIndex = 0;
        internal string GetUnusedSymbol()
        {
            if (nextSymbolIndex >= symbols.Length)
                return "-";
            return symbols[nextSymbolIndex++].ToString();
        }

        private TechTree Generate()
        {
            Random = new Random(Seed);
            nextSymbolIndex = 0;
            Tree = new TechTree();
            Tree.Seed = Seed;

            int numFactories = GenerateFactories();

            GenerateUnits();

            // add prerequisite tech buildings for most unit types
            GenerateTechBuildings(numFactories);

            PositionBuildings();

            return Tree;
        }

        private void AddToSubtree(BuildingType root, BuildingType descendent)
        {
            var children = root.Unlocks.Where(t => t is BuildingType);
            int numChildren = children.Count();

            // sometimes we get a really boring tree, that's just A -> B -> C -> D -> E -> F -> G -> H (etc) with no branching.
            // instead of generating a subtree like that, if the "ancestor chain" is too long, insert further up the tree, instead.
            int chainLengthWithNoSiblings = numChildren == 0 ? 1 : 0;

            if (chainLengthWithNoSiblings > 0)
            {
                var check = root.Prerequisite;
                while (check != null)
                {
                    if (check.Unlocks.Count(t => t is BuildingType) == 1)
                        chainLengthWithNoSiblings++;
                    else
                        break;

                    check = check.Prerequisite;
                }

                if (chainLengthWithNoSiblings > 2)
                {
                    // insert further up the tree instead, and then return
                    int stepsUp = Random.Next(1, chainLengthWithNoSiblings + 1);

                    for (int i = 0; i < stepsUp; i++)
                        root = root.Prerequisite;

                    descendent.Prerequisite = root;
                    return;
                }
            }

            // the more children a node has, the less likely it is to just have this child added directly to it
            // for a node with 1 child, the chances of "falling on" to the next row are 1/3. For one with 2, it's 2/4, for one with 3, it's 3/5,for one with 4, it's 4/6, etc.
            if (numChildren == 0 || Random.Next(numChildren + 2) >= numChildren)
            {
                descendent.Prerequisite = root;
                return;
            }

            // choose a building type that is unlocked by this one
            BuildingType child = children.ElementAt(Random.Next(numChildren)) as BuildingType;

            if (Random.Next(3) == 0)
            {// on a 1/3 chance, insert as a prerequisite of this other child building
                child.Prerequisite = descendent;
                descendent.Prerequisite = root;
            }
            else
            {// otherwise, add as a descendent of this other building
                AddToSubtree(child, descendent);
            }
        }

        private void GenerateUnits()
        {
            // at the point when this is called, every building is a factory

            int tier = 1;
            foreach (var building in Tree.Buildings)
            {
                // each factory building makes 3-4 different types of unit
                GenerateUnitType(building, UnitType.Role.Fighter, tier);
                GenerateUnitType(building, UnitType.Role.Fighter, tier);
                GenerateUnitType(building, UnitType.Role.Hybrid, tier);
                GenerateUnitType(building, UnitType.Role.Caster, tier);
                tier++;
            }
        }

        private UnitType GenerateUnitType(BuildingType building, UnitType.Role role, int tier)
        {
            UnitType unit;
            do
            {
                unit = UnitGenerator.Generate(this, role, tier);
            } while (unit == null);

            unit.BuiltBy = building;
            unit.Prerequisite = building;
            Tree.Units.Add(unit);
            return unit;
        }

        private int GenerateFactories()
        {
            int numFactories;
            switch (TreeComplexity)
            {
                case Complexity.Simple:
                    numFactories = 2;
                    break;
                case Complexity.Normal:
                    numFactories = 3;
                    break;
                case Complexity.Complex:
                    numFactories = 4;
                    break;
                default:
                    throw new ArgumentException("Unexpected Complexity value: " + TreeComplexity);
            }

            BuildingType prev = null;
            for (int i = 0; i < numFactories; i++)
            {
                BuildingType building = BuildingGenerator.GenerateFactory(this, i);
                Tree.Buildings.Add(building);

                if (prev != null)
                    building.Prerequisite = prev;
                prev = building;
            }

            return numFactories;
        }

        private void GenerateTechBuildings(int numFactories)
        {
            int tier = 0;
            for (int i = 0; i < numFactories; i++)
            {
                var building = Tree.Buildings[i];
                BuildingType prevPrerequisite = null;
                bool first = true;

                foreach (var unit in building.Builds)
                {
                    if (first)
                    {// the first unit type built by a building never has any prerequisites
                        first = false;
                        continue;
                    }

                    // give this unit type a 1 in 3 chance of sharing a prerequisite with its predecessor
                    if (prevPrerequisite != null & Random.Next(3) == 0)
                    {
                        unit.Prerequisite = prevPrerequisite;
                        continue;
                    }

                    // generate a new tech building to be this unit type's prerequisite
                    BuildingType techBuilding = BuildingGenerator.GenerateTechBuilding(this, tier);
                    Tree.Buildings.Add(techBuilding);
                    prevPrerequisite = unit.Prerequisite = techBuilding;

                    // insert that into the tech tree somewhere in the factory's subtree
                    AddToSubtree(building, techBuilding);
                }
                tier++;
            }
        }

        private void PositionBuildings()
        {
            BuildingType root = Tree.Buildings[0];
            SetRowRecursive(0, root);

            int maxRow = Tree.Buildings.Max(b => b.DisplayRow);
            int mostChildren = Tree.Buildings.Max(b => b.Unlocks.Where(u => u is BuildingType).Count());

            SpreadColumnsRecursive(root, maxRow, mostChildren);

            // contract the columns as far as possible. If any node jumps over a neighbour, we MAY be left with un-contracted spaces, so contract everything again.
            while (ContractColumns())
                ;
        }

        private void SetRowRecursive(int row, BuildingType b)
        {
            b.DisplayRow = row;
            foreach (BuildingType child in b.Unlocks.Where(u => u is BuildingType))
                SetRowRecursive(row + 1, child);
        }

        private void SpreadColumnsRecursive(BuildingType b, int maxRows, int maxChildren)
        {
            int childSpacing = (int)Math.Pow(maxChildren, maxRows - b.DisplayRow - 1);

            var children = b.Unlocks.Where(u => u is BuildingType);
            int childNum = 0;

            foreach (BuildingType child in children)
            {
                child.DisplayColumn = b.DisplayColumn + (childNum++ * childSpacing);
                SpreadColumnsRecursive(child, maxRows, maxChildren);
            }
        }

        private bool ContractColumns()
        {
            var buildings = Tree.Buildings.Where(b => b.Prerequisite != null).OrderByDescending(b => b.DisplayRow).ThenBy(b => b.DisplayColumn);
            
            foreach (var building in buildings)
            {
                // shift this building (and its subtree) as far left as we can, until it is in-line with its parent, or is blocked
                int shift = DetermineMaxSubtreeLeftShift(building, building.DisplayColumn - building.Prerequisite.DisplayColumn);
                if (shift > 0)
                    ShiftSubtreeLeft(building, shift);
            }

            bool anyJumped = false;
            foreach (var building in buildings)
            {
                if (building.Unlocks.FirstOrDefault(u => u is BuildingType) != null)
                    continue;

                // it might be possible for a childless building to "jump" past a building that is blocking it
                int shift = DetermineJumpLeftShift(building);
                if (shift > 0)
                {
                    anyJumped = true;
                    ShiftSubtreeLeft(building, shift);
                }
            }

            // now shift ALL buildings so that the left-most one is in column 0
            var minCol = Tree.Buildings.Min(b => b.DisplayColumn);
            if (minCol != 0)
                foreach (var b in Tree.Buildings)
                    b.DisplayColumn -= minCol;

            return anyJumped;
        }

        private int DetermineMaxSubtreeLeftShift(BuildingType b, int maxShift)
        {
            if (maxShift == 0)
                return 0;

            // first, see how far left this building can go
            int dist = DetermineAvailableLeftShift(b, maxShift, 0);

            // then, see how far left its left-most descendents can go
            BuildingType leftChild = b.Unlocks.FirstOrDefault(u => u is BuildingType) as BuildingType;
            if (leftChild == null)
                return dist;

            if (dist == 0)
                return 0;

            // return which of the above two is the smallest
            dist = Math.Min(dist, DetermineMaxSubtreeLeftShift(leftChild as BuildingType, dist));

            return dist;
        }

        private int DetermineAvailableLeftShift(BuildingType b, int maxShift, int dist)
        {
            do
            {
                var collision = FindBuilding(b.DisplayRow, b.DisplayColumn - dist - 1);
                if (collision != null)
                    break;
                dist++;
            } while (dist < maxShift);

            return dist;
        }

        private int DetermineJumpLeftShift(BuildingType b)
        {
            var maxShift = b.DisplayColumn - b.Prerequisite.DisplayColumn;

            if (maxShift >= 2)
            {
                var collision = FindBuilding(b.DisplayRow, b.DisplayColumn - 2);
                if (collision == null)
                {
                    int dist = 2;

                    // could now possibly keep moving further left
                    dist = DetermineAvailableLeftShift(b, maxShift, dist);

                    return dist;
                }
            }

            return 0;
        }

        private BuildingType FindBuilding(int row, int column)
        {
            return Tree.Buildings.SingleOrDefault(b => b.DisplayRow == row && b.DisplayColumn == column);
        }

        private void ShiftSubtreeLeft(BuildingType b, int distance)
        {
            b.DisplayColumn -= distance;
            var children = b.Unlocks.Where(u => u is BuildingType);

            foreach (BuildingType child in children)
            {
                ShiftSubtreeLeft(child, distance);
            }
        }

        public enum Complexity
        {
            Simple = 1,
            Normal = 2,
            Complex = 3,
        }
    }
}
