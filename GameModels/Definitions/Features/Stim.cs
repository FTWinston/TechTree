using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Stim : StatusEffectFeature<Stimmed>
    {
        public Stim(int duration, int initialHealthDrain, int extraPoints)
        {
            Effect.Duration = duration;
            Effect.InitialHealthDrain = initialHealthDrain;
            Effect.ExtraPoints = extraPoints;
        }

        public Stim(Dictionary<string, int> data)
        {
            Effect.Duration = data["duration"];
            Effect.InitialHealthDrain = data["initialDrain"];
            Effect.ExtraPoints = data["extraPoints"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "duration", Effect.Duration },
                { "initialDrain", Effect.InitialHealthDrain },
                { "extraPoints", Effect.ExtraPoints },
            });
        }

        public const string TypeID = "own health drain";

        public override string Name => "Drain Own Health";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(Effect.GetDescription());

                if (Effect.InitialHealthDrain > 0)
                {
                    sb.Append(", at the expense of ");
                    sb.Append(Effect.InitialHealthDrain);
                    sb.Append(" hitpoints");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "⚷";
    }
}
