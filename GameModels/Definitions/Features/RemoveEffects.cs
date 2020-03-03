using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class RemoveEffects : EntityTargettedFeature
    {
        public RemoveEffects(int range)
        {
            Range = range;
        }

        public RemoveEffects(string name, string symbol, Dictionary<string, int> data)
        {
            Range = data["range"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData()
            {
                { "range", Range },
            });
        }

        public const string TypeID = "remove effects";

        protected override string Identifier => TypeID;

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

        public override TargetingOptions AllowedTargets => TargetingOptions.AnyOwner | TargetingOptions.Units;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            foreach (var unit in target.Units)
                unit.RemoveAllEffects();

            return true;
        }
    }
}
