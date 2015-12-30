using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.CellEffects
{
    public class DamageOverTime : ICellEffect
    {
        public int Duration { get; internal set; }
        public int DamageMin { get; internal set; }
        public int DamageMax { get; internal set; }

        public void BeforeFirstTick(Cell target) { }
        public void AfterLastTick(Cell target) { }

        public void Tick(Cell target)
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
            sb.Append(" damage per turn to units in this cell, for ");
            sb.Append(Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
    }
}
