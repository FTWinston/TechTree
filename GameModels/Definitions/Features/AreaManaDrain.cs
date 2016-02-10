using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class AreaManaDrain : TargettedFeature
    {
        public override string Name { get { return "Mana Drain"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Drains all mana from everything in a ");
            sb.Append(Radius);
            sb.Append(" tile radius,");

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
        public override string Symbol { get { return "♊"; } }
        public int Radius { get; internal set; }

        public AreaManaDrain(int range, int radius, int damageMin, int damageMax)
        {
            Range = range;
            Radius = radius;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
