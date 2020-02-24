using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class BuildingView : EntityView<Building>
    {
        public BuildingView(Player viewPlayer, Building building)
            : base(viewPlayer, building)
        {
            
        }

        public override string Type => "building";
    }
}
