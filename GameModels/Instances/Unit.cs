using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class Unit : Entity<UnitType>
    {
        public Unit(Player p, UnitType type)
            : base(p, type)
        {

        }
    }
}
