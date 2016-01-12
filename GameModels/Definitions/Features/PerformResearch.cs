using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
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
        
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Unlocks the ");
            sb.Append(Research.Name);
            sb.Append(" feature for ");
            sb.Append(Research.Unlocks.First().EntityDefinition.Name);

            return sb.ToString();
        }

        public Research Research { get; private set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
