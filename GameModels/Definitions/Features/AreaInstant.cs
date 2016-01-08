using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class AreaInstant : TargettedFeature
    {
        public override string Name { get { return "Area Instant"; } }
        public override string GetDescription()
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
        public override string Symbol { get { return "⚵"; } }
        public int Radius { get; internal set; }
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }

        public AreaInstant(int range, int radius, int damageMin, int damageMax)
        {
            Range = range;
            Radius = radius;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
