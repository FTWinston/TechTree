using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.StatusEffects
{
    public class Healing : IStatusEffect
    {
        public int Duration { get; internal set; }
        public int DamageMin { get; internal set; }
        public int DamageMax { get; internal set; }

        public void BeforeFirstTick(Entity target) { }
        public void AfterLastTick(Entity target) { }

        public void Tick(Entity target)
        {
            throw new NotImplementedException();
        }

        public string Name { get { return "Healing"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Restores ");
            sb.Append(DamageMin);
            if (DamageMin != DamageMax)
            {
                sb.Append("-");
                sb.Append(DamageMax);
            }
            sb.Append(" health per turn, for ");
            sb.Append(Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
    }
}
