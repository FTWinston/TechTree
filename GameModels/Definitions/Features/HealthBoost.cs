using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HealthBoost : TargettedStatusEffectFeature<StatusEffects.HealthBoost>
    {
        public HealthBoost(int range, int duration, int healthBoost, int armorBoost)
        {
            Range = range;

            Effect.Duration = duration;
            Effect.ExtraHealth = healthBoost;
            Effect.ExtraArmor = armorBoost;
        }

        public HealthBoost(Dictionary<string, int> data)
        {
            Range = data["range"];
            Effect.Duration = data["duration"];
            Effect.ExtraHealth = data["health"];
            Effect.ExtraArmor = data["armor"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "range", Range },
                { "duration", Effect.Duration },
                { "health", Effect.ExtraHealth },
                { "armor", Effect.ExtraArmor },
            });
        }

        public const string TypeID = "health boost";

        public override string Name => "Health Boost";

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Effect.GetDescription());
                sb.Append(", to a unit");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "⚚";

        public override TargetingOptions AllowedTargets => TargetingOptions.FriendlyOwner | TargetingOptions.Units;
    }
}
