using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class TargettedDoT : TargettedStatusEffectFeature<DamageOverTime>
    {
        public TargettedDoT(int range, int duration, int damageMin, int damageMax)
        {
            Range = range;
            EffectInstance.Duration = duration;
            EffectInstance.DamageMin = damageMin;
            EffectInstance.DamageMax = damageMax;
        }
        
        public override string Name => "Targetted DoT";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(EffectInstance.GetDescription());
                sb.Append(", to an enemy");

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

        public override string Symbol => "♅";

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
