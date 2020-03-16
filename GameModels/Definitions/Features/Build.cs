using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using FeatureData = System.Collections.Generic.Dictionary<string, int>;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Build : Feature
    {
        public Build(uint id, string name, string symbol, UnitType unit)
            : base(id, name, symbol)
        {
            Unit = unit;
        }

        public Build(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            int unitTypeID = data["unit"];
            //Unit = something();
            throw new NotImplementedException();
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("unit", (int)Unit.ID);
            return data;
        }

        internal const string TypeID = "build";

        protected override string TypeIdentifier => TypeID;

        public override FeatureMode Mode { get { return FeatureMode.Purchased; } }

        public override FeatureState DetermineState(Entity entity)
        {
            return entity.Owner.Resources.HasValue(Unit.Cost)
                ? FeatureState.CanTrigger
                : FeatureState.Disabled;
        }

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
            var data = GetEntityData(entity);
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
