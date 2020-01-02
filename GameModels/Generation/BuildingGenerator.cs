using GameModels.Definitions;
using System;

namespace GameModels.Generation
{
    static class BuildingGenerator
    {
        /*
        public static BuildingType GenerateCommandBuilding(OldTreeGenerator gen)
        {
            string symbol = OldTreeGenerator.commandBuildingSymbol.ToString();
            BuildingType building = new BuildingType()
            {
                Name = "Command Post " + symbol,
                Symbol = symbol,
                VisionRange = 2,
            };

            Random r = gen.Random;

            building.MineralCost = r.Next(250, 401).RoundNearest(25);
            building.GasCost = 0;

            building.Health = r.Next(226, 451).RoundNearest(10);
            building.Armor = r.Next(1, 3);

            building.BuildTime = r.Next(3, 6);

            return building;
        }

        public static BuildingType GenerateFactory(OldTreeGenerator gen, int factoryNumber)
        {
            string symbol = gen.AllocateBuildingSymbol();
            BuildingType building = new BuildingType()
            {
                Name = "Factory " + symbol,
                Symbol = symbol,
                VisionRange = 2,
            };
            
            Random r = gen.Random;

            building.MineralCost = (r.Next(100, 201) + r.Next(20,51) * factoryNumber).RoundNearest(25);
            building.GasCost = (r.Next(25, 101) * factoryNumber).RoundNearest(25);

            building.Health = (r.Next(125, 201) + r.Next(25, 50) * factoryNumber).RoundNearest(10);
            building.Armor = r.Next(1, 3) + r.Next(0, factoryNumber + 1);

            building.BuildTime = r.Next(2, 4) + r.Next(0, factoryNumber * 2 + 1);

            return building;
        }

        public static BuildingType GenerateTechBuilding(OldTreeGenerator gen, int factoryNumber)
        {
            string symbol = gen.AllocateBuildingSymbol();
            BuildingType building = new BuildingType()
            {
                Name = "Tech building " + symbol,
                Symbol = symbol,
                VisionRange = 2,
            };

            Random r = gen.Random;

            building.MineralCost = (r.Next(0, 101) + 100 + r.Next(20, 51) * factoryNumber).RoundNearest(25);
            building.GasCost = (r.Next(50, 126) * factoryNumber).RoundNearest(25);

            building.Health = (r.Next(50, 126) + r.Next(15, 35) * factoryNumber).RoundNearest(10);
            building.Armor = r.Next(1, 3) + r.Next(0, factoryNumber);

            building.BuildTime = r.Next(2, 4) + r.Next(0, factoryNumber + 3);

            return building;
        }
        */
    }
}
