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
            EffectInstance.Duration = duration;
            EffectInstance.DamagePerTurn = damagePerTurn;
            EffectInstance.ManaPerHitpoint = manaPerHitpoint;
        }

        public override string Name => "Drain Own Health";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drains ");
                sb.Append(EffectInstance.DamagePerTurn);
                sb.Append(" per turn");

                if (EffectInstance.Duration > 1)
                {
                    sb.Append(", over ");
                    sb.Append(EffectInstance.Duration);
                    sb.Append(" turns");
                }

                sb.Append(", restoring ");
                sb.Append(EffectInstance.ManaPerHitpoint.ToString("n1"));
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
