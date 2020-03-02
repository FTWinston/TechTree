using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Blind : TargettedStatusEffectFeature<Blinded>
    {
        public Blind(int range, int duration)
        {
            Range = range;
            Effect.Duration = duration;
        }

        public Blind(Dictionary<string, int> data)
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

        public const string TypeID = "blind";

        public override string Name => "Blind";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("Reduces the vision radius to zero, for an enemy");
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
                    sb.Append(" turns");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "☊";

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
