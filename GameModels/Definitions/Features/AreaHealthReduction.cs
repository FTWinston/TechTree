using GameModels.Definitions.CellEffects;
using GameModels.Definitions.StatusEffects;
using GameModels.Instances;
using System;
using System.Text;

namespace GameModels.Definitions.Features
{
    public class AreaHealthReduction : TargettedStatusEffectFeature<ReducedHealth>
    {
        public override string Name { get { return "Health Reduction"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Reduces the maximum health of");
            
            if (Radius != 1)
            {
                sb.Append(" units in a ");
                sb.Append(Radius);
                sb.Append(" tile radius,");
            }
            else
                sb.Append(" a unit");

            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            sb.Append(" by up to ");
            sb.Append(EffectInstance.ReductionMax);

            if (EffectInstance.Duration > 0)
            {
                sb.Append(" for ");
                sb.Append(EffectInstance.Duration);
                sb.Append(" turns");
            }

            if (EffectInstance.MinRemainingHitpoints > 0)
            {
                sb.Append(", never dropping below ");
                sb.Append(EffectInstance.MinRemainingHitpoints);
                sb.Append(EffectInstance.MinRemainingHitpoints == 1 ? " hitpoint" : " hitpoints");
            }
            return sb.ToString();
        }
        public override string Symbol { get { return "♓"; } }
        public int Radius { get; internal set; }

        public AreaHealthReduction(int range, int radius, int duration, int hitpointsDrained, int minHitpoints, int drainPerTurn)
        {
            Range = range;
            Radius = radius;

            EffectInstance.Duration = duration;
            EffectInstance.ReductionMax = hitpointsDrained;
            EffectInstance.MinRemainingHitpoints = minHitpoints;
            EffectInstance.HealthDrainPerTurn = drainPerTurn;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return target.Owner != user.Owner;
        }
    }
}
