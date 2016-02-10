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
    public class TargettedDoT : TargettedStatusEffectFeature<DamageOverTime>
    {
        public override string Name { get { return "Targetted DoT"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(EffectInstance.GetDescription());
            sb.Append(", to an enemy");

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
        public override string Symbol { get { return "♅"; } }

        public TargettedDoT(int range, int duration, int damageMin, int damageMax)
        {
            Range = range;
            EffectInstance.Duration = duration;
            EffectInstance.DamageMin = damageMin;
            EffectInstance.DamageMax = damageMax;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner != target.Owner;
        }
    }
}
