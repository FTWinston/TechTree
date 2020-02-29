using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaDetection : TargettedFeature
    {
        public AreaDetection(int range, int radius, int duration)
        {
            Range = range;
            Radius = radius;
            Duration = duration;
        }

        public override string Name => "Detection";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

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
        }

        public override string Symbol => "☼";

        public int Radius { get; }

        public int Duration { get; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
