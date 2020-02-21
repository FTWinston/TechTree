using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class Building : Entity<BuildingType>
    {
        public Building(uint id, Player p, BuildingType type, Cell location)
            : base(id, p, type, location)
        {

        }
    }
}
