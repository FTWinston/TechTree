using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Landmine : ActivatedFeature
    {
        public Landmine(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int damageMin, int damageMax)
            : base(name, symbol, manaCost, limitedUses, cooldown)
        {
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public Landmine(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            DamageMin = data["damageMin"];
            DamageMax = data["damageMax"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("damageMin", DamageMin);
            data.Add("damageMax", DamageMax);
            return data;
        }

        internal const string TypeID = "landmine";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drops an invisible mine, which deals ");
                sb.Append(DamageMin);
                if (DamageMin != DamageMax)
                {
                    sb.Append("-");
                    sb.Append(DamageMax);
                }
                sb.Append(" damage to the next unit to enter this tile");

                return sb.ToString();
            }
        }

        public int DamageMin { get; }

        public int DamageMax { get; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
