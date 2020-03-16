using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Suicide : StatusEffectFeature<Exploding>
    {
        public Suicide(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int duration, int damageMin, int damageMax, int damageDistance)
            : base(id, name, symbol, manaCost, limitedUses, cooldown)
        {
            Effect.Duration = duration;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
            Effect.DamageDistance = damageDistance;
        }

        public Suicide(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Effect.Duration = data["duration"];
            Effect.DamageMin = data["damageMin"];
            Effect.DamageMax = data["damageMax"];
            Effect.DamageDistance = data["damageDistance"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            data.Add("damageMin", Effect.DamageMin);
            data.Add("damageMax", Effect.DamageMax);
            data.Add("damageDistance", Effect.DamageDistance);
            return data;
        }

        internal const string TypeID = "suicide";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Causes this unit to explode");

                if (Effect.Duration > 0)
                {
                    sb.Append(" after ");
                    sb.Append(Effect.Duration);
                    sb.Append(Effect.Duration == 1 ? " turn" : " turns");
                }

                sb.Append(", dealing ");
                sb.Append(Effect.DamageMin);

                if (Effect.DamageMin != Effect.DamageMax)
                {
                    sb.Append("-");
                    sb.Append(Effect.DamageMax);
                }

                sb.Append(" damage to units up to ");
                sb.Append(Effect.DamageDistance);
                sb.Append(" tiles away");
                return sb.ToString();
            }
        }
    }
}
