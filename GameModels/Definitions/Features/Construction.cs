using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Construction : ActivatedFeature
    {
        public Construction(string name, string symbol)
            : base(name, symbol, 0, null, null) { }

        public Construction(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data) { }

        internal const string TypeID = "construct";

        protected override string Identifier => TypeID;

        public override string Description => "Constructs buildings";

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            // TODO: don't think we need this, but then how does construction work?
            throw new NotImplementedException();
        }
    }
}
