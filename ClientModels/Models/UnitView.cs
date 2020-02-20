using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class UnitView : EntityView<Unit>
    {
        public UnitView(Unit unit)
            : base(unit)
        {
            
        }

        public override string Type => "unit";
    }
}
