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
        public DrainOwnHealth(int duration, int damagePerTurn, float manaPerHitpoint)
        {
            Effect.Duration = duration;
            Effect.DamagePerTurn = damagePerTurn;
            Effect.ManaPerHitpoint = manaPerHitpoint;
        }

        public const string TypeID = "drain own health";

        public override string Type => TypeID;

        public override string Name => "Drain Own Health";

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

        public override string Symbol => "♃";

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
