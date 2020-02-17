using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Stim : SelfStatusEffectFeature<Stimmed>
    {
        public override string Name { get { return "Drain Own Health"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(EffectInstance.GetDescription());

            if (EffectInstance.InitialHealthDrain > 0)
            {
                sb.Append(", at the expense of ");
                sb.Append(EffectInstance.InitialHealthDrain);
                sb.Append(" hitpoints");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "⚷"; } }

        public Stim(int duration, int initialHealthDrain, int extraPoints)
        {
            EffectInstance.Duration = duration;
            EffectInstance.InitialHealthDrain = initialHealthDrain;
            EffectInstance.ExtraPoints = extraPoints;
        }
    }
}
