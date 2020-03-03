using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Wall : TargettedCellEffectFeature<CellEffects.Wall>
    {
        public Wall(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range, int duration)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Effect.Duration = duration;
        }

        public Wall(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Effect.Duration = data["duration"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            return data;
        }

        internal const string TypeID = "wall";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Prevents ground units from passing through a cell");

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
    }
}
