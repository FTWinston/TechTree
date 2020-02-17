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
    public class Freeze : TargettedStatusEffectFeature<Frozen>
    {
        public override string Name { get { return "Freeze"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Prevents units");
            
            if (Radius != 1)
            {
                sb.Append(" in a ");
                sb.Append(Radius);
                sb.Append(" tile radius,");
            }

            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            sb.Append(" from moving or taking any actions");

            if (EffectInstance.Duration > 0)
            {
                sb.Append(", for ");
                sb.Append(EffectInstance.Duration);
                sb.Append(" turns");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "⚴"; } }
        public int Radius { get; internal set; }

        public Freeze(int range, int radius, int duration)
        {
            Range = range;
            Radius = radius;
            EffectInstance.Duration = duration;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner != target.Owner;
        }

        public override bool Validate(EntityType type)
        {
            // an entity type should only have one of freeze, slow, immobilize
            return type.Features.FirstOrDefault(f => f is Slow) == null
                && type.Features.FirstOrDefault(f => f is Immobilize) == null;
        }
    }
}
