using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Wall : TargettedCellEffectFeature<CellEffects.Wall>
    {
        public Wall(int range, int duration)
        {
            Range = range;
            Effect.Duration = duration;
        }

        public const string TypeID = "wall";

        public override string Type => TypeID;

        public override string Name => "Wall";

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

        public override string Symbol => "❒";
    }
}
