using ObjectiveStrategy.GameModels.Instances;
using Newtonsoft.Json;
using ObjectiveStrategy.GameModels.Extensions;

using FeatureData = System.Collections.Generic.Dictionary<string, int>;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Build : Feature
    {
        public Build(UnitType unit)
        {
            Unit = unit;
        }

        public override FeatureType Type { get { return FeatureType.Purchased; } }

        public override FeatureState DetermineState(Entity entity)
        {
            return entity.Owner.Resources.HasValue(Unit.Cost)
                ? FeatureState.CanTrigger
                : FeatureState.Disabled;
        }

        public override string Name { get { return "Build: " + Unit.Name; } }
        public override string Symbol { get { return Unit.Symbol; } }

        public override string Description => Unit.WriteCost();
        
        [JsonIgnore]
        public UnitType Unit { get; private set; }

        private const string buildingFeatureKey = "building";

        protected override bool CanTrigger(Entity entity, Cell target, FeatureData data)
        {
            if (!entity.Owner.Resources.HasValue(Unit.Cost))
                return false;

            if (data.TryGetValue(buildingFeatureKey, out _))
                return false; // already building, cannot continue

            return true;
        }

        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            data[buildingFeatureKey] = Unit.BuildTime;
            return true;
        }

        protected override void AfterTriggered(Entity entity, FeatureData data)
        {
            entity.Owner.Resources.SubtractValue(Unit.Cost);
        }

        public override void StartTurn(Entity entity)
        {
            var data = GetData(entity);
            if (!data.TryGetValue(buildingFeatureKey, out var turnsLeft))
                return;

            turnsLeft--;

            if (turnsLeft > 0)
            {
                data[buildingFeatureKey] = turnsLeft - 1;
                return;
            }
            
            // TODO: actually create unit
        }
    }
}
