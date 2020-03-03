using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_AOE_ManaDrain : EffectToggleFeature<AreaInvisible>
    {
        public Cloaking_AOE_ManaDrain(string name, string symbol, int activateManaCost, int manaCostPerTurn, int radius)
            : base(name, symbol, activateManaCost, manaCostPerTurn)
        {
            Radius = radius;
        }

        public Cloaking_AOE_ManaDrain(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Radius = data["radius"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            return data;
        }

        internal const string TypeID = "toggleable aoe cloak";

        protected override string Identifier => TypeID;

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        public int Radius { get; }

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
        */
    }
}
