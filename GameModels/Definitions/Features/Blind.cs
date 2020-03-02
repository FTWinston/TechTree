using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
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

        public const string TypeID = "blind";

        public override string Type => TypeID;

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
