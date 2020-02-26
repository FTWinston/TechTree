using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class Unit : Entity<UnitType>
    {
        public Unit(uint id, Player p, UnitType type, Cell location)
            : base(id, p, type, location)
        {
            MovementRemaining = type.MoveRange;
        }

        public int MovementRemaining { get; set; }
    }
}
