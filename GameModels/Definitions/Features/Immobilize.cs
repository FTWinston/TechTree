using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Immobilize : TargettedStatusEffectFeature<Immobilized>
    {
        public Immobilize(int range, int radius, int duration)
        {
            Range = range;
            Radius = radius;
            Effect.Duration = duration;
        }

        public Immobilize(string name, string symbol, Dictionary<string, int> data)
        {
            Range = data["range"];
            Radius = data["radius"];
            Effect.Duration = data["duration"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData()
            {
                { "range", Range },
                { "radius", Radius },
                { "duration", Effect.Duration },
            });
        }

        public const string TypeID = "immobilize";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("Prevents units");

                if (Radius != 1)
                {
                    sb.Append(" in a ");
                    sb.Append(Radius);
                    sb.Append(" tile radius,");
                }

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(" from moving");

                if (Effect.Duration > 0)
                {
                    sb.Append(", for ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                return sb.ToString();
            }
        }

        public int Radius { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
