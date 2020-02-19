using ObjectiveStrategy.GameModels.Instances;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class PerformResearch : Feature
    {
        public PerformResearch(Research research)
        {
            Research = research;
        }

        public override Feature.InteractionMode Mode { get { return InteractionMode.Purchased; } }
        public override bool UsesMana { get { return false; } }

        public override string Name { get { return "Research: " + Research.Name; } }
        public override string Symbol { get { return Research.Symbol; } }
        
        protected override string GetDescription()
        {
            return Research.Description;
        }

        [JsonIgnore]
        public Research Research { get; private set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
