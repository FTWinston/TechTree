using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
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

        public DrainOwnHealth(Dictionary<string, int> data)
        {
            Effect.Duration = data["range"];
            Effect.DamagePerTurn = data["damagePerTurn"];
            Effect.ManaPerHitpoint = data["manaPerHp"] / 100f;
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "duration", Effect.Duration },
                { "damagePerTurn", Effect.DamagePerTurn },
                { "manaPerHp", (int)(Effect.ManaPerHitpoint * 100) },
            });
        }

        public const string TypeID = "drain own health";

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
