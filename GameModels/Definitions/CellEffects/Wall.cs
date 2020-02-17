using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.CellEffects
{
    public class Wall : ICellEffect
    {
        public int Duration { get; internal set; }

        public void BeforeFirstTick(Cell target)
        {
            throw new NotImplementedException();
        }

        public void AfterLastTick(Cell target)
        {
            throw new NotImplementedException();
        }

        public void Tick(Cell target) { }

        public string Name { get { return "Wall"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Prevents ground units from passing through this cell, for ");
            sb.Append(Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
    }
}
