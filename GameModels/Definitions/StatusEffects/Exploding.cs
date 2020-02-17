using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.StatusEffects
{
    public class Exploding : IStatusEffect
    {
        public int Duration { get; internal set; }

        public void BeforeFirstTick(Entity target) { }

        public void AfterLastTick(Entity target)
        {
            throw new NotImplementedException();
        }

        public void Tick(Entity target) { }

        public string Name { get { return "Exploding"; } }
        public int DamageMin { get; internal set; }
        public int DamageMax { get; internal set; }
        public int DamageDistance { get; internal set; }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("This unit will shortly explode, dealing ");
            sb.Append(DamageMin);

            if (DamageMin != DamageMax)
            {
                sb.Append("-");
                sb.Append(DamageMax);
            }

            sb.Append(" damage to units up to ");
            sb.Append(DamageDistance);
            sb.Append(" tiles away");
            return sb.ToString();
        }
    }
}
