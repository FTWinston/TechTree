using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class PersonalTeleport : TargettedFeature
    {
        public override string Name { get { return "Teleport"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Teleports this unit to a cell ");

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
        public override string Symbol { get { return "☆"; } }

        public PersonalTeleport(int range)
        {
            Range = range;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
