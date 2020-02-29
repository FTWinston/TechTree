using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaShield : TargettedCellEffectFeature<CellEffects.Shield>
    {
        public AreaShield(int range, int radius, int duration, int extraHealth, int extraArmor)
        {
            Range = range;
            Radius = radius;
            EffectInstance.ExtraHealth = extraHealth;
            EffectInstance.ExtraArmor = extraArmor;
            EffectInstance.Duration = duration;
        }

        public override string Name => "Area Shield";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Adds ");

                if (EffectInstance.ExtraHealth > 0)
                {
                    sb.Append(EffectInstance.ExtraHealth);
                    sb.Append(" extra health");
                }

                if (EffectInstance.ExtraArmor > 0)
                {
                    if (EffectInstance.ExtraHealth > 0)
                        sb.Append(" and ");
                    sb.Append(EffectInstance.ExtraArmor);
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
                sb.Append(EffectInstance.Duration);
                sb.Append(" turns");

                return sb.ToString();
            }
        }

        public override string Symbol => "♎";

        public int Radius { get; }
    }
}
