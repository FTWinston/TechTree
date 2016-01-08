using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class AreaDetection : TargettedFeature
    {
        public override string Name { get { return "Detection"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Reveals all units");
            
            if (Radius != 1)
            {
                sb.Append(" in a ");
                sb.Append(Radius);
                sb.Append(" tile radius,");
            }
            else
                sb.Append(" in a cell");

            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            if (Duration > 0)
            {
                sb.Append(" for ");
                sb.Append(Duration);
                sb.Append(Duration == 1 ? " turn" : " turns");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "☼"; } }
        public int Radius { get; internal set; }
        public int Duration { get; internal set; }

        public AreaDetection(int range, int radius, int duration)
        {
            Range = range;
            Radius = radius;
            Duration = duration;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
