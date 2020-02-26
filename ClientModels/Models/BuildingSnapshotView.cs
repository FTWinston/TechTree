using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class BuildingSnapshotView : EntityView
    {
        public BuildingSnapshotView(BuildingSnapshot building)
        {
            Building = building;
        }

        private BuildingSnapshot Building { get; }

        public override string Type => "building";

        public override uint ID => Building.ID;

        public override int Owner => Building.Owner.ID;

        public override int Health => Building.Health;

        public override int MaxHealth => Building.MaxHealth;

        public override int Mana => 0;

        public override int MaxMana => 0;

        public override int Armor => Building.Armor;

        public override FeatureView[] Features => new FeatureView[] { };
    }
}
