using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Carrier : PassiveFeature
    {
        public Carrier(uint id, string name, string symbol, int capacity)
            : base(id, name, symbol)
        {
            Capacity = capacity;
        }

        public Carrier(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            Capacity = data["capacity"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("capacity", Capacity);
            return data;
        }

        internal const string TypeID = "transport";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Carries up to {Capacity} units";

        private int Capacity { get; }
    }
}
