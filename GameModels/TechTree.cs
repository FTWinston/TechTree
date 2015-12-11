using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels
{
    public class TechTree
    {
        public List<BuildingType> Buildings { get; private set; }
        public List<UnitType> Units { get; private set; }
        public List<Research> Research { get; private set; }
        public int Seed { get; internal set; }

        public TechTree()
        {
            Buildings = new List<BuildingType>();
            Units = new List<UnitType>();
            Research = new List<Research>();
        }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Buildings:");
            sb.AppendLine();
            foreach (var building in Buildings)
            {
                building.Describe(sb);
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Units:");
            sb.AppendLine();
            foreach (var unit in Units)
            {
                unit.Describe(sb);
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Research:");
            sb.AppendLine();
            foreach (var research in Research)
            {
                research.Describe(sb);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
