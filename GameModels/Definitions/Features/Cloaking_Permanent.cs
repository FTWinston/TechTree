using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_Permanent : PassiveFeature
    {
        public Cloaking_Permanent(string name, string symbol)
            : base(name, symbol)
        {
            Effect = new Invisible();

            // TODO: work out how to actually apply this effect to units
        }

        public Cloaking_Permanent(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol)
        {
            Effect = new Invisible();

            // TODO: work out how to actually apply this effect to units}
        }

        public const string TypeID = "permanent cloak";

        protected override string Identifier => TypeID;

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        private Invisible Effect { get; set; }

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
        */
    }
}
