using GameModels.Definitions.CellEffects;
using GameModels.Instances;
using System;
using System.Text;

namespace GameModels.Definitions.Features
{
    public class AreaDoT : TargettedCellEffectFeature<DamageOverTime>
    {
        public override string Name { get { return "Area DoT"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Deals ");
            sb.Append(EffectInstance.DamageMin);
            if (EffectInstance.DamageMin != EffectInstance.DamageMax)
            {
                sb.Append("-");
                sb.Append(EffectInstance.DamageMax);
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
            sb.Append(EffectInstance.Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
        public override char Appearance { get { return '*'; } }
        public int Radius { get; internal set; }

        public AreaDoT(int range, int radius, int duration, int damageMin, int damageMax)
        {
            Range = range;
            Radius = radius;
            EffectInstance.DamageMin = damageMin;
            EffectInstance.DamageMax = damageMax;
            EffectInstance.Duration = duration;
        }
    }
}
