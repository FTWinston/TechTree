using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Suicide : StatusEffectFeature<Exploding>
    {
        public Suicide(int duration, int damageMin, int damageMax, int damageDistance)
        {
            Effect.Duration = duration;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
            Effect.DamageDistance = damageDistance;
        }

        public override string Name => "Suicide";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Causes this unit to explode");

                if (Effect.Duration > 0)
                {
                    sb.Append(" after ");
                    sb.Append(Effect.Duration);
                    sb.Append(Effect.Duration == 1 ? " turn" : " turns");
                }

                sb.Append(", dealing ");
                sb.Append(Effect.DamageMin);

                if (Effect.DamageMin != Effect.DamageMax)
                {
                    sb.Append("-");
                    sb.Append(Effect.DamageMax);
                }

                sb.Append(" damage to units up to ");
                sb.Append(Effect.DamageDistance);
                sb.Append(" tiles away");
                return sb.ToString();
            }
        }

        public override string Symbol => "♄";
    }
}
