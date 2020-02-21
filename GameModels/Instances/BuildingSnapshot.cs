using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class BuildingSnapshot
    {
        public BuildingSnapshot(Building building)
        {
            ID = building.ID;
            BuildingType = building.Definition;
            Owner = building.Owner;
            Health = building.Health;
            MaxHealth = building.Definition.Health;
            Armor = building.Definition.Armor;
        }

        public BuildingType BuildingType { get; }

        public uint ID { get; }

        public Player Owner { get; }

        public int Health { get; }

        public int MaxHealth { get; }

        public int Armor { get; }
    }
}
