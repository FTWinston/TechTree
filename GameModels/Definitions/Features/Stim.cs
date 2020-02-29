using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Stim : StatusEffectFeature<Stimmed>
    {
        public Stim(int duration, int initialHealthDrain, int extraPoints)
        {
            EffectInstance.Duration = duration;
            EffectInstance.InitialHealthDrain = initialHealthDrain;
            EffectInstance.ExtraPoints = extraPoints;
        }

        public override string Name => "Drain Own Health";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(EffectInstance.GetDescription());

                if (EffectInstance.InitialHealthDrain > 0)
                {
                    sb.Append(", at the expense of ");
                    sb.Append(EffectInstance.InitialHealthDrain);
                    sb.Append(" hitpoints");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "⚷";
    }
}
