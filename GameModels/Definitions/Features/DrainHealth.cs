using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DrainHealth : TargettedStatusEffectFeature<HealthDrain>
    {
        public override string Name { get { return "Drain Health"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Drains ");
            sb.Append(EffectInstance.DamagePerTurn);
            sb.Append(" per turn");

            if (EffectInstance.Duration > 1)
            {
                sb.Append(", over ");
                sb.Append(EffectInstance.Duration);
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
            sb.Append(EffectInstance.ManaPerHitpoint.ToString("n1"));
            sb.Append(" mana to the caster for each hitpoint drained");

            return sb.ToString();
        }
        public override string Symbol { get { return "⛎"; } }

        public DrainHealth(int range, int duration, int damagePerTurn, float manaPerHitpoint)
        {
            Range = range;
            EffectInstance.Duration = duration;
            EffectInstance.DamagePerTurn = damagePerTurn;
            EffectInstance.ManaPerHitpoint = manaPerHitpoint;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return true;
        }
    }
}
