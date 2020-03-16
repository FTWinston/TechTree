using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Burrow : EffectToggleFeature<Burrowed>
    {
        public Burrow(uint id, string name, string symbol, int activatedManaCost, int manaCostPerTurn)
            : base(id, name, symbol, activatedManaCost, manaCostPerTurn) { }

        public Burrow(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data) { }

        internal const string TypeID = "burrow";

        protected override string TypeIdentifier => TypeID;

        public override string Description => "Prevents this unit from moving, attacking, or being seen by enemy units that lack the [detector] feature";

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null;
        }
        */
    }
}
