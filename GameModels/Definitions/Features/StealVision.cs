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

        public StealVision(string name, string symbol, Dictionary<string, int> data)
        {
            Range = data["range"];
            Effect.Duration = data["duration"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData()
            {
                { "range", Range },
                { "duration", Effect.Duration },
            });
        }

        public const string TypeID = "steal vision";

        protected override string Identifier => TypeID;

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

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
