using ObjectiveStrategy.GameModels.Definitions.CellEffects;
using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaHealthReduction : TargettedStatusEffectFeature<ReducedHealth>
    {
        public AreaHealthReduction(int range, int radius, int duration, int hitpointsDrained, int minHitpoints, int drainPerTurn)
        {
            Range = range;
            Radius = radius;

            EffectInstance.Duration = duration;
            EffectInstance.ReductionMax = hitpointsDrained;
            EffectInstance.MinRemainingHitpoints = minHitpoints;
            EffectInstance.HealthDrainPerTurn = drainPerTurn;
        }

        public override string Name => "Health Reduction";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

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

        }

        public override string Symbol => "♓";

        public int Radius { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
