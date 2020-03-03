using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaManaDrain : TargettedFeature
    {
        public AreaManaDrain(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range, int radius, int damageMin, int damageMax)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
        }

        public AreaManaDrain(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Radius = data["radius"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            return data;
        }

        public const string TypeID = "area mana drain";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drains all mana from everything in a ");
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

                return sb.ToString();
            }
        }

        public int Radius { get; internal set; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
