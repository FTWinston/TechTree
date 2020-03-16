using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Possession : EntityTargettedFeature
    {
        public Possession(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        { }

        public Possession(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        { }

        internal const string TypeID = "possession";

        protected override string TypeIdentifier => TypeID;

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
