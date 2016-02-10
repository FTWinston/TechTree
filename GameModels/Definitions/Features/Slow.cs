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
    public class Slow : TargettedStatusEffectFeature<Slowed>
    {
        public override string Name { get { return "Slow"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Reduces by ");
            sb.Append(EffectInstance.ReducedPoints);
            sb.Append(" the number of action points available to units");

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
        public override string Symbol { get { return "♑"; } }
        public int Radius { get; internal set; }

        public Slow(int range, int radius, int duration, int reducedPoints)
        {
            Range = range;
            Radius = radius;
            EffectInstance.Duration = duration;
            EffectInstance.ReducedPoints = reducedPoints;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner != target.Owner;
        }

        public override bool Validate(EntityType type)
        {
            // an entity type should only have one of freeze, slow, immobilize
            return type.Features.FirstOrDefault(f => f is Freeze) == null
                && type.Features.FirstOrDefault(f => f is Immobilize) == null;
        }
    }
}
