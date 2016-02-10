using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class MindControl : EntityTargettedFeature
    {
        public override string Name { get { return "Mind Control"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Takes ownership of an enemy unit");

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
                sb.Append(", for ");
                sb.Append(Duration);
                sb.Append(" turns");
            }

            if (MaxControlRange > 0)
            {
                sb.Append(", as long as it remains within ");
                sb.Append(MaxControlRange);
                sb.Append(" cells of the caster");
            }

            return sb.ToString();
        }
        public override string Symbol { get { return "♍"; } }
        public int Duration { get; private set; }
        public int MaxControlRange { get; private set; }

        public MindControl(int range, int duration, int maxControlRange)
        {
            Range = range;
            Duration = duration;
            MaxControlRange = maxControlRange;
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
