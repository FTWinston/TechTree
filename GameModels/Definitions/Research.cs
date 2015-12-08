using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public class Research
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public char Appearance { get; internal set; }

        public int TurnsToComplete { get; internal set; }

        public List<Feature> Unlocks { get; private set; }

        public Research()
        {
            Unlocks = new List<Feature>();
        }

        internal virtual void Describe(StringBuilder sb)
        {
            sb.AppendLine();
        }
    }
}
