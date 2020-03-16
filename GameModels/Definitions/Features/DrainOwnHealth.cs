using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DrainOwnHealth : StatusEffectFeature<HealthDrain>
    {
        public DrainOwnHealth(uint id, string name, string symbol, int? limitedUses, int? cooldown, int duration, int damagePerTurn, float manaPerHitpoint)
            : base(id, name, symbol, 0, limitedUses, cooldown)
        {
            Effect.Duration = duration;
            Effect.DamagePerTurn = damagePerTurn;
            Effect.ManaPerHitpoint = manaPerHitpoint;
        }

        public DrainOwnHealth(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Effect.Duration = data["range"];
            Effect.DamagePerTurn = data["damagePerTurn"];
            Effect.ManaPerHitpoint = data["manaPerHp"] / 100f;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            data.Add("damagePerTurn", Effect.DamagePerTurn);
            data.Add("manaPerHp", (int)(Effect.ManaPerHitpoint * 100));
            return data;
        }

        internal const string TypeID = "drain own health";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drains ");
                sb.Append(Effect.DamagePerTurn);
                sb.Append(" per turn");

                if (Effect.Duration > 1)
                {
                    sb.Append(", over ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                sb.Append(", restoring ");
                sb.Append(Effect.ManaPerHitpoint.ToString("n1"));
                sb.Append(" mana to the caster for each hitpoint drained");

                return sb.ToString();
            }
        }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
