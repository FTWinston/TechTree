using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels
{
    public class TechTree : BaseTechTree<BuildingType, UnitType>
    {
        public TechTree()
        {

        }

        public TechTree(BaseTechTree copyFrom) : base
        ( // Each player will get a copy of the "main" tech tree, so that upgrades can be applied separately.
            buildings: copyFrom.AllBuildings.ToDictionary(x => x.Key, x => new BuildingType(x.Value)),
            units: copyFrom.AllUnits.ToDictionary(x => x.Key, x => new UnitType(x.Value)),
            research: new Dictionary<uint, Research>(copyFrom.Research) // No need to copy these, simply remove them once they are completed.
        ) {
            
        }
    }
}
