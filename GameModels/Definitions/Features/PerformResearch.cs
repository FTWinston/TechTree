using ObjectiveStrategy.GameModels.Instances;
using Newtonsoft.Json;
using System.Collections.Generic;
using ObjectiveStrategy.GameModels.Extensions;
using ObjectiveStrategy.GameModels.Serialization;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class PerformResearch : Feature
    {
        public PerformResearch(Research research)
        {
            Research = research;
        }

        public PerformResearch(string name, string symbol, Dictionary<string, int> data)
        {
            int researchID = data["research"];
            Research = something;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData()
            {
                { "research", Research.ID },
            });
        }

        public const string TypeID = "research";

        protected override string Identifier => TypeID;

        public override FeatureMode Mode => FeatureMode.Purchased;

        public override string Description => Research.Description;

        [JsonIgnore]
        public Research Research { get; private set; }

        public override FeatureState DetermineState(Entity entity)
        {
            var data = GetEntityData(entity);

            return CanTrigger(entity, entity.Location, data)
                ? FeatureState.CanTrigger
                : FeatureState.Disabled;
        }

        private const string researchingFeatureKey = "progress";

        protected override bool CanTrigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            // TODO: research being in progress ANYWHERE invalidates it everywhere.
            // Alternatively, research completing anywhere cancels it everywhere.

            if (entity.Owner.CompletedResearch.Contains(Research))
                return false;

            if (!entity.Owner.Resources.HasValue(Research.Cost))
                return false;

            if (data.TryGetValue(researchingFeatureKey, out _))
                return false; // already building, cannot continue

            return true;
        }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            data[researchingFeatureKey] = Research.BuildTime;
            return true;
        }

        public override void StartTurn(Entity entity)
        {
            var data = GetEntityData(entity);
            if (!data.TryGetValue(researchingFeatureKey, out var turnsLeft))
                return;

            turnsLeft--;

            if (turnsLeft > 0)
            {
                data[researchingFeatureKey] = turnsLeft - 1;
                return;
            }

            entity.Owner.CompletedResearch.Add(Research);

            // TODO: cancel this research everywhere else

            // TODO: unlock any unit or building

            // TODO: apply any feature

            // TODO: this seems like a job for a research service
        }
    }
}
