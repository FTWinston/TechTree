using ObjectiveStrategy.GameModels.Definitions.Features;
using ObjectiveStrategy.GameModels.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public class Research : Purchasable
    {
        public string Description { get; internal set; }

        public BuildingType PerformedAt { get; private set; }
        public List<Feature> Unlocks { get; private set; }

        public Research(params Feature[] features)
            : base(null) // TODO: this is screwed up
        {
            /*
            Name = features.First().Name;
            Symbol = features.First().Symbol;
            */
            Unlocks = new List<Feature>(features);
        }
        
        /*
        internal static Research CreateForFeature(OldTreeGenerator gen, BuildingType researchedBy, Feature feature, int tier)
        {
            var research = new Research(feature)
            {
                BuildTime = gen.Random.Next(Math.Min(3, tier), tier + 3),
                MineralCost = (gen.Random.Next(80, 185) + gen.Random.Next(20, 40) * tier).RoundNearest(25),
                GasCost = (gen.Random.Next(35, 80) + gen.Random.Next(32, 55) * tier).RoundNearest(25),
                PerformedAt = feature.EntityDefinition.Prerequisite,
            };

            // researchedBy.AddFeature(new PerformResearch(research));
            return research;
        }
        */
    }
}
