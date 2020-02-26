using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class FeatureView
    {
        public FeatureView(Entity entity, Feature feature)
        {
            Entity = entity;
            Feature = feature;
        }

        private Entity Entity { get; }

        private Feature Feature { get; }

        public string Name => Feature.Name;

        public string Description => Feature.Description;

        public string Symbol => Feature.Symbol;

        public FeatureState State => Feature.DetermineState(Entity);
    }
}
