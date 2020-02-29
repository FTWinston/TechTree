using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DrainHealth : TargettedStatusEffectFeature<HealthDrain>
    {
        public DrainHealth(int range, int duration, int damagePerTurn, float manaPerHitpoint)
        {
            Range = range;
            Effect.Duration = duration;
            Effect.DamagePerTurn = damagePerTurn;
            Effect.ManaPerHitpoint = manaPerHitpoint;
        }

        public override string Name => "Drain Health";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drains ");
                sb.Append(Effect.DamagePerTurn);
                sb.Append(" per turn");

                if (Effect.Duration > 1)
                {
                    sb.Append(", over ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                sb.Append(" from a unit");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(", restoring ");
                sb.Append(Effect.ManaPerHitpoint.ToString("n1"));
                sb.Append(" mana to the caster for each hitpoint drained");

                return sb.ToString();
            }
        }

        public override string Symbol => "⛎";

        public override TargetingOptions AllowedTargets => TargetingOptions.AnyOwner | TargetingOptions.Units;
    }
}
