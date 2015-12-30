using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.StatusEffects
{
    public class AreaInvisible : IStatusEffect
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

        public string Name { get { return "Invisible"; } }
        public int Radius { get; internal set; }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Friendly units within ");
            sb.Append(Radius);
            sb.Append(" tiles cannot be seen by enemy units without the [detector] feature");
            return sb.ToString();
        }
    }
}
