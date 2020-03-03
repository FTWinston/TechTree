using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Carrier : PassiveFeature
    {
        public Carrier(string name, string symbol, int capacity)
            : base(name, symbol)
        {
            Capacity = capacity;
        }

        public Carrier(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol)
        {
            Capacity = data["capacity"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("capacity", Capacity);
            return data;
        }

        public const string TypeID = "transport";

        protected override string Identifier => TypeID;

        public override string Description => $"Carries up to {Capacity} units";

        private int Capacity { get; }
    }
}
