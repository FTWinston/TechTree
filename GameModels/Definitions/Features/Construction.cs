using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Construction : ActivatedFeature
    {
        public override string Name => "Build";

        public override string Description => "Constructs buildings";

        public override string Symbol => "⚒";

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            // TODO: don't think we need this, but then how does construction work?
            throw new NotImplementedException();
        }
    }
}
