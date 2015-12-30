using GameModels.Definitions;
using GameModels.Instances;
using GameModels.Definitions.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class HealOverTime : TargettedStatusEffectFeature<Healing>
    {
        public override string Name { get { return "Healing over Time"; } }
        public override string GetDescription()
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
        public override char Appearance { get { return '+'; } }

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
    }
}
