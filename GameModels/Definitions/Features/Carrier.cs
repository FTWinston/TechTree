using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Carrier : PassiveFeature
    {
        public Carrier(int capacity)
        {
            Capacity = capacity;
        }

        public Carrier(Dictionary<string, int> data)
            : this(data["capacity"]) { }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "capacity", Capacity },
            });
        }

        public const string TypeID = "transport";

        public override string Name => "Carrier";

        public override string Description => $"Carries up to {Capacity} units";

        public override string Symbol => "⚖";

        private int Capacity { get; }
    }
}
