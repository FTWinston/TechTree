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
            Random r = seed == null ? new Random() : new Random(seed.Value);

            var generator = new TreeGenerator()
            {
                Random = r,
                TreeComplexity = complexity
            };
            return generator.Generate();
        }

        private TreeGenerator() { }
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
            nextSymbolIndex = 0;
            Tree = new TechTree();

            int numFactories = GenerateFactories();

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

            // add prerequisite tech buildings for most unit types
            tier = 0;
            for (int i = 0; i < numFactories; i++ )
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

            return Tree;
        }

        private void AddToSubtree(BuildingType root, BuildingType descendent)
        {
            int numBuildingUnlocks = 0;
            foreach (var type in root.Unlocks)
                if (type is BuildingType)
                    numBuildingUnlocks++;

            if (numBuildingUnlocks == 0 || Random.Next(3) == 0)
            {
                descendent.Prerequisite = root;
                return;
            }

            // find another building type that is unlocked by this root
            BuildingType other = null;

            int iBuildingUnlock = 0, selectedUnlock = Random.Next(numBuildingUnlocks);
            foreach (var type in root.Unlocks)
                if (type is BuildingType)
                {
                    if (iBuildingUnlock == selectedUnlock)
                    {
                        other = type as BuildingType;
                        break;
                    }
                    iBuildingUnlock++;
                }

            if (Random.Next(3) == 0)
            {
                // insert as a prerequisite of this other building we've found
                other.Prerequisite = descendent;
                descendent.Prerequisite = root;
            }
            else
            {// add as a descendent of this other building
                AddToSubtree(other, descendent);
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

        public enum Complexity
        {
            Simple = 1,
            Normal = 2,
            Complex = 3,
        }
    }
}
