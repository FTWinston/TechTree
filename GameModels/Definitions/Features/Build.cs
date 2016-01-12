using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class Build : Feature
    {
        public Build(UnitType unit)
        {
            Unit = unit;
        }

        public override Feature.InteractionMode Mode { get { return InteractionMode.Purchased; } }
        public override bool UsesMana { get { return false; } }

        public override string Name { get { return "Build: " + Unit.Name; } }
        public override string Symbol { get { return Unit.Symbol; } }
        
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Builds a new ");
            sb.Append(Unit.Name);

            if (Unit.MineralCost > 0 || Unit.VespineCost > 0)
            {
                sb.Append(", costing ");

                if (Unit.MineralCost > 0)
                {
                    sb.Append(Unit.MineralCost);
                    sb.Append(" minerals");

                    if (Unit.VespineCost > 0)
                        sb.Append(" and ");
                }

                if (Unit.VespineCost > 0)
                {
                    sb.Append(Unit.VespineCost);
                    sb.Append(" vespine");
                }
            }

            if (Unit.SupplyCost != 0)
            {
                if (Unit.SupplyCost > 0)
                    sb.Append(", using ");
                else
                    sb.Append(", generating ");

                sb.Append(Math.Abs(Unit.SupplyCost));
                sb.Append(" supply");
            }

            sb.Append(", taking ");
            sb.Append(Unit.BuildTime);
            sb.Append(Unit.BuildTime == 1 ? " turn" : "turns");

            return sb.ToString();
        }

        public UnitType Unit { get; private set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
