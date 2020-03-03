using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HealthBoost : TargettedStatusEffectFeature<StatusEffects.HealthBoost>
    {
        public HealthBoost(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int duration, int healthBoost, int armorBoost)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Effect.Duration = duration;
            Effect.ExtraHealth = healthBoost;
            Effect.ExtraArmor = armorBoost;
        }

        public HealthBoost(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Effect.Duration = data["duration"];
            Effect.ExtraHealth = data["health"];
            Effect.ExtraArmor = data["armor"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            data.Add("health", Effect.ExtraHealth);
            data.Add("armor", Effect.ExtraArmor);
            return data;
        }

        internal const string TypeID = "health boost";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Effect.GetDescription());
                sb.Append(", to a unit");

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

        public override TargetingOptions AllowedTargets => TargetingOptions.FriendlyOwner | TargetingOptions.Units;
    }
}
