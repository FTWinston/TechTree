using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class Building : Entity<BuildingType>
    {
        public Building(Player p, BuildingType type)
            : base(p, type)
        {

        }
    }
}
