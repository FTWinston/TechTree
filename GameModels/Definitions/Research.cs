using GameModels.Definitions.Features;
using GameModels.Generation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public class Research : Purchasable
    {
        public string Description { get; internal set; }

        [JsonIgnore]
        public BuildingType PerformedAt { get; private set; }

        [JsonProperty("PerformedAt", NullValueHandling = NullValueHandling.Ignore)]
        public int? PerformedAtNumber { get { if (PerformedAt == null) return null; return PerformedAt.BuildingNumber; } }
        
        [JsonIgnore]
        public List<Feature> Unlocks { get; private set; }

        internal int ResearchNumber { get; set; }

        public Research(params Feature[] features)
        {
            Name = features.First().Name;
            Symbol = features.First().Symbol;
            Unlocks = new List<Feature>(features);
        }

        internal static Research CreateForFeature(TreeGenerator gen, BuildingType researchedBy, Feature feature, int tier)
        {
            var research = new Research(feature) {
                BuildTime = gen.Random.Next(Math.Min(3, tier), tier + 3),
                MineralCost = (gen.Random.Next(80, 185) + gen.Random.Next(20, 40) * tier).RoundNearest(25),
                VespineCost = (gen.Random.Next(35, 80) + gen.Random.Next(32, 55) * tier).RoundNearest(25),
                PerformedAt = feature.EntityDefinition.Prerequisite,
                ResearchNumber = gen.Tree.Research.Count
            };

            gen.Tree.Research.Add(research);
            researchedBy.AddFeature(new PerformResearch(research));
            return research;
        }
    }
}
