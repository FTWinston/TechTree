using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaInstant : TargettedFeature
    {
        public AreaInstant(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int radius, int damageMin, int damageMax)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public AreaInstant(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Radius = data["radius"];
            DamageMin = data["damageMin"];
            DamageMax = data["damageMax"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            data.Add("damageMin", DamageMin);
            data.Add("damageMax", DamageMax);
            return data;
        }

        internal const string TypeID = "area damage";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Deals ");
                sb.Append(DamageMin);
                if (DamageMin != DamageMax)
                {
                    sb.Append("-");
                    sb.Append(DamageMax);
                }
                sb.Append(" damage per turn to enemies");

                if (Radius != 1)
                {
                    sb.Append(" in a ");
                    sb.Append(Radius);
                    sb.Append(" tile radius,");
                }

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

        public int Radius { get; }

        public int DamageMin { get; }

        public int DamageMax { get; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            // TODO: need access to battlefield to get cells in range
            throw new NotImplementedException();
        }
    }
}
