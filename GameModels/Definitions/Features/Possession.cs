using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Possession : EntityTargettedFeature
    {
        public Possession(int range)
        {
            Range = range;
        }

        public override string Name => "Possession";

        public override string Description
        {
            get
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

                sb.Append(", removing this unit in the process");

                return sb.ToString();
            }
        }

        public override string Symbol => "♌";

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
