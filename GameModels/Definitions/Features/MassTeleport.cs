using GameModels.Definitions;
using GameModels.Definitions.StatusEffects;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class MassTeleport : TargettedStatusEffectFeature<Teleporting>
    {
        public override string Name { get { return "Mass Teleport"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Teleports ");
            sb.Append(FriendlyOnly ? "friendly" : "all");
            sb.Append(" units in a ");

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

            sb.Append(", to this unit");

            return sb.ToString();
        }
        public override string Symbol { get { return "⚝"; } }
        public int Radius { get; private set; }
        public bool FriendlyOnly { get; private set; }

        public MassTeleport(int range, int radius, bool friendlyOnly)
        {
            Range = range;
            Radius = radius;
            FriendlyOnly = friendlyOnly;
        }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            throw new NotImplementedException();
        }
    }
}
