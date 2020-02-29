using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class RemoveEffects : EntityTargettedFeature
    {
        public RemoveEffects(int range)
        {
            Range = range;
        }

        public override string Name => "Remove Effects";

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Removes all status effects from a target unit");

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
        }

        public override string Symbol => "⚜";

        public override TargetingOptions AllowedTargets => TargetingOptions.AnyOwner | TargetingOptions.Units;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            foreach (var unit in target.Units)
                unit.RemoveAllEffects();

            return true;
        }
    }
}
