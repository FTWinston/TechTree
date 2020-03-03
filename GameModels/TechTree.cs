using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels
{
    [JsonConverter(typeof(TechTreeConverter))]
    public class TechTree : BaseTechTree<BuildingType, UnitType>
    {
        public TechTree()
        {

        }

        public TechTree(BaseTechTree copyFrom) : base
        ( // Each player will get a copy of the "main" tech tree, so that upgrades can be applied separately.
            buildings: copyFrom.AllBuildings.ToDictionary(x => x.Key, x => new BuildingType(x.Value)),
            units: copyFrom.AllUnits.ToDictionary(x => x.Key, x => new UnitType(x.Value)),
            research: new Dictionary<uint, Research>(copyFrom.Research) // No need to copy these, simply remove them once they are completed. TODO: actually do so a definition can be used by multiple games, and possibly updated in-between
        ) {
            
        }
    }
}
