using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.StatusEffects
{
    public class DamageOverTime : IStatusEffect
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

        public string Name { get { return "DoT"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Deals ");
            sb.Append(DamageMin);
            if (DamageMin != DamageMax)
            {
                sb.Append("-");
                sb.Append(DamageMax);
            }
            sb.Append(" damage per turn, for ");
            sb.Append(Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
    }
}
