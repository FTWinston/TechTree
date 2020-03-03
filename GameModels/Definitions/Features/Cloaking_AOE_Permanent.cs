using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_AOE_Permanent : PassiveFeature
    {
        public Cloaking_AOE_Permanent(string name, string symbol, int radius)
            : base(name, symbol)
        {
            Radius = radius;
            Effect = new AreaInvisible();

            // TODO: work out how to actually apply this effect to units
        }

        public Cloaking_AOE_Permanent(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol)
        {
            Radius = data["radius"];
            Effect = new AreaInvisible();

            // TODO: work out how to actually apply this effect to units
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            return data;
        }

        internal const string TypeID = "permanent aoe cloak";

        protected override string Identifier => TypeID;

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        public int Radius { get; }

        private AreaInvisible Effect { get; }

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
        */
    }
}
