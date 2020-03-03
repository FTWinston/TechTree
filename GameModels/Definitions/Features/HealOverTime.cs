using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HealOverTime : TargettedStatusEffectFeature<Healing>
    {
        public HealOverTime(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int duration, int damageMin, int damageMax)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Effect.Duration = duration;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
        }

        public HealOverTime(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
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

        internal const string TypeID = "heal over time";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
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

        public override TargetingOptions AllowedTargets => TargetingOptions.SameOwner | TargetingOptions.Units;

        /*
        public override bool Validate(EntityType type)
        {
            // don't allow on a type that also has heal-over-time
            return type.Features.FirstOrDefault(f => f is InstantHeal) == null;
        }
        */
    }
}
