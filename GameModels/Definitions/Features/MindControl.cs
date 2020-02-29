using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class MindControl : EntityTargettedFeature
    {
        public MindControl(int range, int duration, int maxControlRange)
        {
            Range = range;
            Duration = duration;
            MaxControlRange = maxControlRange;
        }

        public override string Name => "Mind Control";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

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
        }

        public override string Symbol => "♍";

        public int Duration { get; }

        public int MaxControlRange { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
