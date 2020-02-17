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
    public class Blind : TargettedStatusEffectFeature<Blinded>
    {
        public override string Name { get { return "Blind"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Reduces the vision radius to zero, for an enemy");
            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            if (EffectInstance.Duration > 0)
            {
                sb.Append(", for ");
                sb.Append(EffectInstance.Duration);
                sb.Append(" turns");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "☊"; } }

        public Blind(int range, int duration)
        {
            Range = range;
            EffectInstance.Duration = duration;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner != target.Owner;
        }
    }
}
