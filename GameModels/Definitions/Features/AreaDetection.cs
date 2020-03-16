using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaDetection : TargettedFeature
    {
        public AreaDetection(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range, int radius, int duration)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
            Duration = duration;
        }

        public AreaDetection(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Radius = data["radius"];
            Duration = data["duration"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            data.Add("radius", Radius);
            data.Add("duration", Duration);

            return data;
        }

        internal const string TypeID = "reveal";
        
        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Reveals all units");

                if (Radius != 1)
                {
                    sb.Append(" in a ");
                    sb.Append(Radius);
                    sb.Append(" tile radius,");
                }
                else
                    sb.Append(" in a cell");

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
                    sb.Append(" for ");
                    sb.Append(Duration);
                    sb.Append(Duration == 1 ? " turn" : " turns");
                }

                return sb.ToString();
            }
        }

        public int Radius { get; }

        public int Duration { get; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
