using GameModels;
using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Generation
{
    static class BuildingGenerator
    {
        public static BuildingType GenerateFactory(TreeGenerator gen, int factoryNumber)
        {
            BuildingType building = new BuildingType()
            {
                Name = "Factory " + gen.GetUnusedSymbol(),
                VisionRange = 2,
            };
            
            Random r = gen.Random;

            building.MineralCost = (r.Next(0, 101) + 100 + r.Next(20,51) * factoryNumber).RoundNearest(25);
            building.VespineCost = (r.Next(25, 101) * factoryNumber).RoundNearest(25);

            building.Health = (r.Next(125, 201) + r.Next(25, 50) * factoryNumber).RoundNearest(10);
            building.Armor = r.Next(1, 3) + r.Next(0, factoryNumber + 1);

            building.BuildTime = r.Next(2, 4) + r.Next(0, factoryNumber * 2 + 1);

            return building;
        }

        public static BuildingType GenerateTechBuilding(TreeGenerator gen, int factoryNumber)
        {
            BuildingType building = new BuildingType()
            {
                Name = "Tech building " + gen.GetUnusedSymbol(),
                VisionRange = 2,
            };

            Random r = gen.Random;

            building.MineralCost = (r.Next(0, 101) + 100 + r.Next(20, 51) * factoryNumber).RoundNearest(25);
            building.VespineCost = (r.Next(50, 126) * factoryNumber).RoundNearest(25);

            building.Health = (r.Next(50, 126) + r.Next(15, 35) * factoryNumber).RoundNearest(10);
            building.Armor = r.Next(1, 3) + r.Next(0, factoryNumber);

            building.BuildTime = r.Next(2, 4) + r.Next(0, factoryNumber + 3);

            return building;
        }
    }
}
