using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class TargettedDoT : TargettedStatusEffectFeature<DamageOverTime>
    {
        public TargettedDoT(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int duration, int damageMin, int damageMax)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Effect.Duration = duration;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
        }
        public TargettedDoT(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Effect.Duration = data["duration"];
            Effect.DamageMin = data["damageMin"];
            Effect.DamageMax = data["damageMax"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            data.Add("damageMin", Effect.DamageMin);
            data.Add("damageMax", Effect.DamageMax);
            return data;
        }

        internal const string TypeID = "targetted dot";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(Effect.GetDescription());
                sb.Append(", to an enemy");

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

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Units;
    }
}
