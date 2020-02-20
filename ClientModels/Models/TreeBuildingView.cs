using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class TreeBuildingView : TreeEntityView<BuildingType>
    {
        public TreeBuildingView(TechTree techTree, BuildingType buildingType)
            : base(buildingType)
        {
            Builds = techTree.Units
                .Where(kvp => buildingType.Builds.Contains(kvp.Key))
                .Select(kvp => new TreeUnitView(kvp.Value))
                .ToArray();

            Researches = techTree.Research
                .Where(kvp => kvp.Value.PerformedAt == buildingType)
                .Select(kvp => new ResearchView(kvp.Value))
                .ToArray();
        }

        public uint ID => Item.ID;

        public int Row => Item.DisplayRow;

        public int Col => Item.DisplayColumn;

        public List<uint> Unlocks => Item.Unlocks;

        public TreeUnitView[] Builds { get; }

        public ResearchView[] Researches { get; }
    }
}
