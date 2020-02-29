using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HealthBoost : TargettedStatusEffectFeature<StatusEffects.HealthBoost>
    {
        public HealthBoost(int range, int duration, int healthBoost, int armorBoost)
        {
            Range = range;

            EffectInstance.Duration = duration;
            EffectInstance.ExtraHealth = healthBoost;
            EffectInstance.ExtraArmor = armorBoost;
        }

        public override string Name => "Health Boost";

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(EffectInstance.GetDescription());
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
