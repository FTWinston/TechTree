using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.StatusEffects
{
    public class StolenVision : IStatusEffect
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

        public string Name { get { return "Stolen Vision"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("An enemy can see everything that this unit sees");

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
