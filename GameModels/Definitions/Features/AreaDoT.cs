using ObjectiveStrategy.GameModels.Definitions.CellEffects;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaDoT : TargettedCellEffectFeature<DamageOverTime>
    {
        public AreaDoT(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int radius, int duration, int damageMin, int damageMax)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
            Effect.Duration = duration;
        }

        public AreaDoT(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Radius = data["radius"];
            Effect.Duration = data["duration"];
            Effect.DamageMin = data["damageMin"];
            Effect.DamageMax = data["damageMax"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            data.Add("duration", Effect.Duration);
            data.Add("damageMin", Effect.DamageMin);
            data.Add("damageMax", Effect.DamageMax);
            return data;
        }

        internal const string TypeID = "area dot";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Deals ");
                sb.Append(Effect.DamageMin);
                if (Effect.DamageMin != Effect.DamageMax)
                {
                    sb.Append("-");
                    sb.Append(Effect.DamageMax);
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

                sb.Append(", for ");
                sb.Append(Effect.Duration);
                sb.Append(" turns");

                return sb.ToString();
            }
        }

        public int Radius { get; }
    }
}
