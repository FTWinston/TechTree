using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Suicide : StatusEffectFeature<Exploding>
    {
        public Suicide(int duration, int damageMin, int damageMax, int damageDistance)
        {
            EffectInstance.Duration = duration;
            EffectInstance.DamageMin = damageMin;
            EffectInstance.DamageMax = damageMax;
            EffectInstance.DamageDistance = damageDistance;
        }

        public override string Name => "Suicide";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Causes this unit to explode");

                if (EffectInstance.Duration > 0)
                {
                    sb.Append(" after ");
                    sb.Append(EffectInstance.Duration);
                    sb.Append(EffectInstance.Duration == 1 ? " turn" : " turns");
                }

                sb.Append(", dealing ");
                sb.Append(EffectInstance.DamageMin);

                if (EffectInstance.DamageMin != EffectInstance.DamageMax)
                {
                    sb.Append("-");
                    sb.Append(EffectInstance.DamageMax);
                }

                sb.Append(" damage to units up to ");
                sb.Append(EffectInstance.DamageDistance);
                sb.Append(" tiles away");
                return sb.ToString();
            }
        }

        public override string Symbol => "♄";
    }
}
