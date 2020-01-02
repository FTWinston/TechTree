using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModels.Generation
{
    public class OldTreeGenerator
    {
        /*
        internal TechTree Tree { get; private set; }

        private TechTree Generate()
        {
            Random = new Random(Seed);
            nextBuildingSymbol = nextUnitSymbol = 0;
            Tree = new TechTree(Seed);

            GenerateFactories();
            List<BuildingType> factories = new List<BuildingType>(Tree.Buildings);

            AddCommandBuilding();

            GenerateUnitStubs(factories);

            // add prerequisite tech buildings for most unit types
            GenerateTechBuildings(factories);

            PositionBuildings();

            PopulateUnits();

            return Tree;
        }

        private void AddCommandBuilding()
        {
            BuildingType building = BuildingGenerator.GenerateCommandBuilding(this);
            Tree.Buildings.First().Prerequisite = building;
            Tree.Buildings.Insert(0, building);

            UnitType unit = UnitGenerator.GenerateWorker(this);
            unit.BuiltBy = unit.Prerequisite = building;
            Tree.Units.Add(unit);
        }

        private void GenerateUnitStubs(List<BuildingType> factories)
        {
            foreach (var building in factories)
            {
                // each factory building makes 4 different types of unit
                for (int i = 0; i < 4; i++)
                {
                    UnitType unit = UnitGenerator.GenerateStub(this);
                    unit.BuiltBy = unit.Prerequisite = building;
                    Tree.Units.Add(unit);
                }
            }
        }

        private void GenerateFactories()
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
        }
        */
    }
}
