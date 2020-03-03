using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class MindControl : EntityTargettedFeature
    {
        public MindControl(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int? duration, int? maxControlRange)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Duration = duration;
            MaxControlRange = maxControlRange;
        }

        public MindControl(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            if (data.TryGetValue("duration", out var duration))
                Duration = Duration;

            if (data.TryGetValue("controlRange", out var range))
                MaxControlRange = range;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            
            if (Duration.HasValue)
                data.Add("duration", Duration.Value);

            if (MaxControlRange.HasValue)
                data.Add("controlRange", MaxControlRange.Value);
            return data;
        }

        internal const string TypeID = "mind control";

        protected override string Identifier => TypeID;

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

        public int? Duration { get; }

        public int? MaxControlRange { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
