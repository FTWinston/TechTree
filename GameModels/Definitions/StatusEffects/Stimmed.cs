using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.StatusEffects
{
    public class Stimmed : IStatusEffect
    {
        public int InitialHealthDrain { get; internal set; }
        public int ExtraPoints { get; internal set; }
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

        public string Name { get { return "Stimmed"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Increases the number of action points available to this unit by ");
            sb.Append(ExtraPoints);

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
