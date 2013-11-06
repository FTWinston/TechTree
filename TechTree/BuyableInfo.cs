using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTree
{
    public class BuyableInfo
    {
        public string Name { get; set; }
        
        public int MineralCost { get; set; }
        public int GasCost { get; set; }

        public float BuildTime { get; set; }
    }

    public class EntityInfo : BuyableInfo
    {
        public int Health { get; set; }
        public int Armor { get; set; }
        public int Mana { get; set; }
        public int SupplyCost { get; set; }
    }

    public class BuildingInfo : EntityInfo
    {
        
    }

    public class UnitInfo : EntityInfo
    {

    }

    public class ResearchInfo : BuyableInfo
    {

    }
}
