using ObjectiveStrategy.GameModels.Definitions.CellEffects;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaDoT : TargettedCellEffectFeature<DamageOverTime>
    {
        public AreaDoT(int range, int radius, int duration, int damageMin, int damageMax)
        {
            Range = range;
            Radius = radius;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
            Effect.Duration = duration;
        }

        public const string TypeID = "area dot";

        public override string Type => TypeID;

        public override string Name => "Area DoT";

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

        public override string Symbol => "♋";

        public int Radius { get; }
    }
}
