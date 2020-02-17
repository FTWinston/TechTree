using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.StatusEffects
{
    public class HealthDrain : IStatusEffect
    {
        public int Duration { get; internal set; }
        public int DamagePerTurn { get; internal set; }
        public float ManaPerHitpoint { get; internal set; }

        public void BeforeFirstTick(Entity target) { }
        public void AfterLastTick(Entity target) { }

        public void Tick(Entity target)
        {
            throw new NotImplementedException();
        }

        public string Name { get { return "Health Drain"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Drains ");
            sb.Append(DamagePerTurn);
            sb.Append(" per turn");

            if (Duration > 1)
            {
                sb.Append(", over ");
                sb.Append(Duration);
                sb.Append(" turns");
            }

            sb.Append(", restoring ");
            sb.Append(ManaPerHitpoint.ToString("n1"));
            sb.Append(" mana to the caster for each hitpoint drained");

            return sb.ToString();
        }
    }
}
