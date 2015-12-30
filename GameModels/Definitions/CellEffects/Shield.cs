using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.CellEffects
{
    public class Shield : ICellEffect
    {
        public int Duration { get; internal set; }
        public int ExtraHealth { get; set; }
        public int ExtraArmor { get; set; }

        public void BeforeFirstTick(Cell target)
        {
            throw new NotImplementedException();
        }

        public void AfterLastTick(Cell target)
        {
            throw new NotImplementedException();
        }

        public void Tick(Cell target) { }

        public string Name { get { return "Shield"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Adds ");

            if (ExtraHealth > 0)
            {
                sb.Append(ExtraHealth);
                sb.Append(" extra health");
            }

            if (ExtraArmor > 0)
            {
                if (ExtraHealth > 0)
                    sb.Append(" and ");
                sb.Append(ExtraArmor);
                sb.Append(" extra armor");
            }

            sb.Append("to units in this cell, for ");
            sb.Append(Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
    }
}
