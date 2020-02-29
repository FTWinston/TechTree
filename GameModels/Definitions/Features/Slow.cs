using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Slow : TargettedStatusEffectFeature<Slowed>
    {
        public Slow(int range, int radius, int duration, int reducedPoints)
        {
            Range = range;
            Radius = radius;
            Effect.Duration = duration;
            Effect.ReducedPoints = reducedPoints;
        }

        public override string Name => "Slow";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("Reduces by ");
                sb.Append(Effect.ReducedPoints);
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

                if (Effect.Duration > 0)
                {
                    sb.Append(", for ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "♑";

        public int Radius { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies;
    }
}
