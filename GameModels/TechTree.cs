using GameModels.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace GameModels
{
    public class TechTree : ITechTree
    {
        public TechTree()
        {
            Buildings = new Dictionary<uint, IBuildingType>();

            Units = new Dictionary<uint, IUnitType>();

            Research = new Dictionary<uint, Research>();
        }

        public TechTree(ITechTree copyFrom) // Each player will get a copy of the "main" tech tree, so that upgrades can be applied separately.
        {
            Buildings = copyFrom.Buildings.ToDictionary<KeyValuePair<uint, IBuildingType>, uint, IBuildingType>(b => b.Key, b => new BuildingType(b.Value));

            Units = copyFrom.Units.ToDictionary<KeyValuePair<uint, IUnitType>, uint, IUnitType>(b => b.Key, u => new UnitType(u.Value));

            Research = new Dictionary<uint, Research>(copyFrom.Research); // No need to copy these, simply remove them once they are completed.
        }

        public Dictionary<uint, IBuildingType> Buildings { get; }

        public Dictionary<uint, IUnitType> Units { get; }

        public Dictionary<uint, Research> Research { get; }
    }
}
