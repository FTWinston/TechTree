using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Blind : TargettedStatusEffectFeature<Blinded>
    {
        public Blind(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range, int duration)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Range = range;
            Effect.Duration = duration;
        }

        public Blind(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Range = data["range"];
            Effect.Duration = data["duration"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            return data;
        }

        public const string TypeID = "blind";

        protected override string Identifier => TypeID;

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

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
