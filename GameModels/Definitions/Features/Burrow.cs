using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Burrow : EffectToggleFeature<Burrowed>
    {
        public Burrow(int activateManaCost)
        {
            ActivateManaCost = activateManaCost;
        }

        public Burrow(Dictionary<string, int> data)
            : this(data["activateCost"]) { }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "activateCost", ActivateManaCost },
            });
        }

        public const string TypeID = "burrow";

        public override string Name => "Burrow";

        public override string Description => "Prevents this unit from moving, attacking, or being seen by enemy units that lack the [detector] feature";

        public override string Symbol => "♉";

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
