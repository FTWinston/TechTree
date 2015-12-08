using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Instances
{
    public class Unit : Entity<UnitType>
    {
        public Unit(Player p, UnitType type)
            : base(p, type)
        {

        }
    }
}
