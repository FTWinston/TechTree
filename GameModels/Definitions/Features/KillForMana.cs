using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class KillForMana : EntityTargettedFeature
    {
        public override string Name { get { return "Kill for Mana"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Kills a friendly unit ");
    
            if (Range == 1)
                sb.Append(" 1 tile away");
            else
            {
                sb.Append(" up to ");
                sb.Append(Range);
                sb.Append(" tiles away");
            }

            sb.Append(", and restores ");
            sb.Append(ManaPerHitpoint.ToString("n1"));
            sb.Append(" mana for each hitpoint the killed unit had");
            
            return sb.ToString();
        }
        public override char Appearance { get { return 'x'; } }
        public float ManaPerHitpoint { get; protected set; }

        public KillForMana(int range, float manaPerHitpoint)
        {
            Range = range;
            ManaPerHitpoint = manaPerHitpoint;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return user.Owner == target.Owner;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
