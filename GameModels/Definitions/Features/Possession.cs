using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
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

        public Possession(string name, string symbol, Dictionary<string, int> data)
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

        public const string TypeID = "possession";

        protected override string Identifier => TypeID;

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

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
