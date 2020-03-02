using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaHealthReduction : TargettedStatusEffectFeature<ReducedHealth>
    {
        public AreaHealthReduction(int range, int radius, int duration, int hitpointsDrained, int minHitpoints, int drainPerTurn)
        {
            Range = range;
            Radius = radius;

            Effect.Duration = duration;
            Effect.ReductionMax = hitpointsDrained;
            Effect.MinRemainingHitpoints = minHitpoints;
            Effect.HealthDrainPerTurn = drainPerTurn;
        }

        public AreaHealthReduction(Dictionary<string, int> data)
        {
            Range = data["range"];
            Radius = data["radius"];
            Effect.Duration = data["duration"];
            Effect.ReductionMax = data["maxReduction"];
            Effect.MinRemainingHitpoints = data["minRemainingHp"];
            Effect.HealthDrainPerTurn = data["hpPerTurn"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "range", Range },
                { "radius", Radius },
                { "duration", Effect.Duration },
                { "maxReduction", Effect.ReductionMax },
                { "minRemainingHp", Effect.MinRemainingHitpoints },
                { "hpPerTurn", Effect.HealthDrainPerTurn },
            });
        }

        public const string TypeID = "area health reduction";

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
                sb.Append(Effect.ReductionMax);

                if (Effect.Duration > 0)
                {
                    sb.Append(" for ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                if (Effect.MinRemainingHitpoints > 0)
                {
                    sb.Append(", never dropping below ");
                    sb.Append(Effect.MinRemainingHitpoints);
                    sb.Append(Effect.MinRemainingHitpoints == 1 ? " hitpoint" : " hitpoints");
                }
                return sb.ToString();
            }

        }

        public override string Symbol => "♓";

        public int Radius { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
