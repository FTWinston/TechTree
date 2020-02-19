using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class TreeUnitView : TreeEntityView<UnitType>
    {
        public TreeUnitView(UnitType unitType)
            : base(unitType)
        { }
    }
}
