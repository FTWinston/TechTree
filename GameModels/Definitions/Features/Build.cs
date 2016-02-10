using GameModels.Instances;
using Newtonsoft.Json;
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

            return sb.ToString();
        }

        [JsonIgnore]
        public UnitType Unit { get; private set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
