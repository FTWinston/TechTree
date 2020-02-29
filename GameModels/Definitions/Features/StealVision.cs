using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class StealVision : TargettedStatusEffectFeature<StolenVision>
    {
        public StealVision(int range, int duration)
        {
            Range = range;
            EffectInstance.Duration = duration;
        }

        public override string Name => "Steal Vision";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
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
        }

        public override string Symbol => "⚯";

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
