using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class TargettedInstant : EntityTargettedFeature
    {
        public override string Name { get { return "Targetted Instant"; } }
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
            sb.Append(" damage to an enemy");

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
        public override string Symbol { get { return "♆"; } }
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }

        public TargettedInstant(int range, int damageMin, int damageMax)
        {
            Range = range;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner != target.Owner;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
