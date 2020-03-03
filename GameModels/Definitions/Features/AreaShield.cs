using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaShield : TargettedCellEffectFeature<CellEffects.Shield>
    {
        public AreaShield(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int radius, int duration, int extraHealth, int extraArmor)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
            Effect.Duration = duration;
            Effect.ExtraHealth = extraHealth;
            Effect.ExtraArmor = extraArmor;
        }

        public AreaShield(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Radius = data["radius"];
            Effect.Duration = data["duration"];
            Effect.ExtraHealth = data["health"];
            Effect.ExtraArmor = data["armor"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            data.Add("duration", Effect.Duration);
            data.Add("health", Effect.ExtraHealth);
            data.Add("armor", Effect.ExtraArmor);
            return data;
        }

        internal const string TypeID = "area shield";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Adds ");

                if (Effect.ExtraHealth > 0)
                {
                    sb.Append(Effect.ExtraHealth);
                    sb.Append(" extra health");
                }

                if (Effect.ExtraArmor > 0)
                {
                    if (Effect.ExtraHealth > 0)
                        sb.Append(" and ");
                    sb.Append(Effect.ExtraArmor);
                    sb.Append(" extra armor");
                }

                sb.Append("to units in ");

                if (Radius != 1)
                {
                    sb.Append("cells in a ");
                    sb.Append(Radius);
                    sb.Append(" tile radius,");
                }
                else
                    sb.Append("a cell");

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
