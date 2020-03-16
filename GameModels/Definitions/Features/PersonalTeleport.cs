using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class PersonalTeleport : TargettedFeature
    {
        public PersonalTeleport(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        { }

        public PersonalTeleport(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        { }

        internal const string TypeID = "personal teleport";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Teleports this unit to a cell ");

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

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
