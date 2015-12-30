using GameModels.Definitions.CellEffects;
using GameModels.Instances;
using System;
using System.Text;

namespace GameModels.Definitions.Features
{
    public class Wall : TargettedCellEffectFeature<CellEffects.Wall>
    {
        public override string Name { get { return "Wall"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

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
            sb.Append(EffectInstance.Duration);
            sb.Append(" turns");

            return sb.ToString();
        }
        public override char Appearance { get { return '*'; } }

        public Wall(int range, int duration)
        {
            Range = range;
            EffectInstance.Duration = duration;
        }
    }
}
