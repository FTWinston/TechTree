using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Instances
{
    public class Building : Entity<BuildingType>
    {
        public Building(Player p, BuildingType type)
            : base(p, type)
        {

        }
    }
}
