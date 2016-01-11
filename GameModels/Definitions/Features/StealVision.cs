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
    public class StealVision : TargettedStatusEffectFeature<StolenVision>
    {
        public override string Name { get { return "Steal Vision"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Gains the vision of an enemy unit");

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
                sb.Append(EffectInstance.Duration == 1 ? " turn" : " turns");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "⚯"; } }

        public StealVision(int range, int duration)
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
