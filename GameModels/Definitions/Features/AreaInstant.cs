using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaInstant : TargettedFeature
    {
        public AreaInstant(int range, int radius, int damageMin, int damageMax)
        {
            Range = range;
            Radius = radius;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public override string Name => "Area Instant";

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Deals ");
                sb.Append(DamageMin);
                if (DamageMin != DamageMax)
                {
                    sb.Append("-");
                    sb.Append(DamageMax);
                }
                sb.Append(" damage per turn to enemies");

                if (Radius != 1)
                {
                    sb.Append(" in a ");
                    sb.Append(Radius);
                    sb.Append(" tile radius,");
                }

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                return sb.ToString();
            }
        }

        public override string Symbol => "⚵";

        public int Radius { get; }

        public int DamageMin { get; }

        public int DamageMax { get; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            // TODO: need access to battlefield to get cells in range
            throw new NotImplementedException();
        }
    }
}
