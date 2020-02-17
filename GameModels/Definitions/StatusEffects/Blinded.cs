using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.StatusEffects
{
    public class Blinded : IStatusEffect
    {
        public int Duration { get; internal set; }

        public void BeforeFirstTick(Entity target)
        {
            throw new NotImplementedException();
        }

        public void AfterLastTick(Entity target)
        {
            throw new NotImplementedException();
        }

        public void Tick(Entity target) { }

        public string Name { get { return "Blinded"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Reduces vision range to 0");

            if (Duration > 0)
            {
                sb.Append(", for ");
                sb.Append(Duration);
                sb.Append(" turns");
            }
            return sb.ToString();
        }
    }
}
