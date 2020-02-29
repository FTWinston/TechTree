using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_AOE_ManaDrain : EffectToggleFeature<AreaInvisible>
    {
        public Cloaking_AOE_ManaDrain(int manaCostPerTurn, int activateManaCost, int radius)
        {
            Radius = radius;
            ManaCostPerTurn = manaCostPerTurn;
            ActivateManaCost = activateManaCost;
        }

        public override string Name => "AOE Cloaking";

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        public override string Symbol => "⛲";

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
