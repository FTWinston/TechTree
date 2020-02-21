using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class BuildingView : EntityView<Building>
    {
        public BuildingView(Building building)
            : base(building)
        {
            
        }

        public override string Type => "building";
    }
}
