using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels
{
    public class TechTree
    {
        public List<BuildingType> Buildings { get; private set; }
        public List<UnitType> Units { get; private set; }
        public List<Research> Research { get; private set; }
        public int Seed { get; internal set; }

        public TechTree()
        {
            Buildings = new List<BuildingType>();
            Units = new List<UnitType>();
            Research = new List<Research>();
        }
    }
}
