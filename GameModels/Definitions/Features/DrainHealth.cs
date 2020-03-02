using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
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

        public DrainHealth(Dictionary<string, int> data)
        {
            Range = data["range"];
            Effect.Duration = data["duration"];
            Effect.DamagePerTurn = data["damagePerTurn"];
            Effect.ManaPerHitpoint = data["manaPerHp"] / 100f;
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "range", Range },
                { "duration", Effect.Duration },
                { "damagePerTurn", Effect.DamagePerTurn },
                { "manaPerHp", (int)(Effect.ManaPerHitpoint * 100) },
            });
        }

        public const string TypeID = "drain health";

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
