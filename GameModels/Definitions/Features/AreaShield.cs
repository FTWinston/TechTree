using GameModels.Definitions.CellEffects;
using GameModels.Instances;
using System;
using System.Text;

namespace GameModels.Definitions.Features
{
    public class AreaShield : TargettedCellEffectFeature<CellEffects.Shield>
    {
        public override string Name { get { return "Area Shield"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

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
        public override char Appearance { get { return '*'; } }
        public int Radius { get; internal set; }

        public AreaShield(int range, int radius, int duration, int extraHealth, int extraArmor)
        {
            Range = range;
            Radius = radius;
            EffectInstance.ExtraHealth = extraHealth;
            EffectInstance.ExtraArmor = extraArmor;
            EffectInstance.Duration = duration;
        }
    }
}
