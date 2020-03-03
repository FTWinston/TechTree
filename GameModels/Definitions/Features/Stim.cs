using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Stim : StatusEffectFeature<Stimmed>
    {
        public Stim(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int duration, int initialHealthDrain, int extraPoints)
            : base(name, symbol, manaCost, limitedUses, cooldown)
        {
            Effect.Duration = duration;
            Effect.InitialHealthDrain = initialHealthDrain;
            Effect.ExtraPoints = extraPoints;
        }

        public Stim(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Effect.Duration = data["duration"];
            Effect.InitialHealthDrain = data["initialDrain"];
            Effect.ExtraPoints = data["extraPoints"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            data.Add("initialDrain", Effect.InitialHealthDrain);
            data.Add("extraPoints", Effect.ExtraPoints);
            return data;
        }

        internal const string TypeID = "own health drain";

        protected override string Identifier => TypeID;

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
    }
}
