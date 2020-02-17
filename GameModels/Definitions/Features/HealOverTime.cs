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
    public class HealOverTime : TargettedStatusEffectFeature<Healing>
    {
        public override string Name { get { return "Healing over Time"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(EffectInstance.GetDescription());
            sb.Append(", to a unit");

            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "⚕"; } }

        public HealOverTime(int range, int duration, int damageMin, int damageMax)
        {
            Range = range;
            EffectInstance.Duration = duration;
            EffectInstance.DamageMin = damageMin;
            EffectInstance.DamageMax = damageMax;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner == target.Owner;
        }

        public override bool Validate(EntityType type)
        {
            // don't allow on a type that also has heal-over-time
            return type.Features.FirstOrDefault(f => f is InstantHeal) == null;
        }
    }
}
