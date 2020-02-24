using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class UnitView : EntityView<Unit>
    {
        public UnitView(Player viewPlayer, Unit unit)
            : base(viewPlayer, unit)
        {
            
        }

        public override string Type => "unit";
    }
}
