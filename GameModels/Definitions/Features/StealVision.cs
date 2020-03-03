using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class StealVision : TargettedStatusEffectFeature<StolenVision>
    {
        public StealVision(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int duration)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Effect.Duration = duration;
        }

        public StealVision(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Effect.Duration = data["duration"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            return data;
        }

        internal const string TypeID = "steal vision";

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
