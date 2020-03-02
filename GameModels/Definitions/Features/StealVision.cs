using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class StealVision : TargettedStatusEffectFeature<StolenVision>
    {
        public StealVision(int range, int duration)
        {
            Range = range;
            Effect.Duration = duration;
        }

        public StealVision(Dictionary<string, int> data)
        {
            Range = data["range"];
            Effect.Duration = data["duration"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "range", Range },
                { "duration", Effect.Duration },
            });
        }

        public const string TypeID = "steal vision";

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

                if (Effect.Duration > 0)
                {
                    sb.Append(", for ");
                    sb.Append(Effect.Duration);
                    sb.Append(Effect.Duration == 1 ? " turn" : " turns");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "⚯";

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
