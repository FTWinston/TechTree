using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_ManaDrain : EffectToggleFeature<Invisible>
    {
        public Cloaking_ManaDrain(int manaCostPerTurn, int activateManaCost)
        {
            ManaCostPerTurn = manaCostPerTurn;
            ActivateManaCost = activateManaCost;
        }

        public Cloaking_ManaDrain(Dictionary<string, int> data)
        {
            ManaCostPerTurn = data["manaPerTurn"];
            ActivateManaCost = data["activateCost"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "manaPerTurn", ManaCostPerTurn },
                { "activateCost", ActivateManaCost },
            });
        }

        public const string TypeID = "toggleable cloak";

        public override string Name => "Cloaking";

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        public override string Symbol => "☋";

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
        */
    }
}
