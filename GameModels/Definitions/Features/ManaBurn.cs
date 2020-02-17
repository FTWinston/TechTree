using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class ManaBurn : EntityTargettedFeature
    {
        public override string Name { get { return "Mana Burn"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            if (MaxMana > 0)
            {
                sb.Append("Drains up to ");
                sb.Append(MaxMana);
            }
            else
                sb.Append("Drains all the");

            sb.Append(" mana from an enemy");

            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            sb.Append(", dealing ");
            sb.Append(DamagePerMana.ToString("n1"));
            sb.Append(" damage for each point drained");

            return sb.ToString();
        }
        public override string Symbol { get { return "⚳"; } }
        public int MaxMana { get; protected set; }
        public float DamagePerMana { get; protected set; }

        public ManaBurn(int range, int maxMana, float damagePerMana)
        {
            Range = range;
            MaxMana = maxMana;
            DamagePerMana = damagePerMana;
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
