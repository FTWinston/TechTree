using ObjectiveStrategy.GameModels.Serialization;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Definitions
{
    [JsonConverter(typeof(GameDefinitionConverter))]
    public class GameDefinition
    {
        public GameDefinition(int complexity, int seed, int turnLimit, TechTree techTree, Battlefield battlefield, Objective[] objectives)
        {
            Complexity = complexity;
            Seed = seed;
            TurnLimit = turnLimit;
            TechTree = techTree;
            Battlefield = battlefield;
            Objectives = objectives;
        }

        public int Complexity { get; }

        public int Seed { get; }

        public int TurnLimit { get; }

        public TechTree TechTree { get; }
        
        public Battlefield Battlefield { get; }

        public Objective[] Objectives { get; }
    }
}
