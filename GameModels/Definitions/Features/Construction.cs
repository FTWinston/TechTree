using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.Features;
using ObjectiveStrategy.GameModels.Generation;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Construction : ActivatedFeature
    {
        public override string Name { get { return "Build"; } }
        protected override string GetDescription()
        {
            return "Constructs buildings";
        }

        public override string Symbol { get { return "⚒"; } }
        
        public Construction()
        {

        }

        public override void Activate(Entity user)
        {
            throw new NotImplementedException();
        }
    }
}
