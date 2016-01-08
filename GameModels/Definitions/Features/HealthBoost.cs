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
    public class HealthBoost : TargettedStatusEffectFeature<StatusEffects.HealthBoost>
    {
        public override string Name { get { return "Health Boost"; } }
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
        public override string Symbol { get { return "⚚"; } }

        public HealthBoost(int range, int duration, int healthBoost, int armorBoost)
        {
            Range = range;

            EffectInstance.Duration = duration;
            EffectInstance.ExtraHealth = healthBoost;
            EffectInstance.ExtraArmor = armorBoost;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner == target.Owner;
        }
    }
}
