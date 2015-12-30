using GameModels.Definitions.StatusEffects;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class DrainOwnHealth : SelfStatusEffectFeature<HealthDrain>
    {
        public override string Name { get { return "Drain Own Health"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            
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
        public override char Appearance { get { return 'd'; } }

        public DrainOwnHealth(int duration, int damagePerTurn, float manaPerHitpoint)
        {
            EffectInstance.Duration = duration;
            EffectInstance.DamagePerTurn = damagePerTurn;
            EffectInstance.ManaPerHitpoint = manaPerHitpoint;
        }

        public override void Activate(Entity user)
        {
            throw new NotImplementedException();
        }
    }
}
