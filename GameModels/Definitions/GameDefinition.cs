using ObjectiveStrategy.GameModels.Serialization;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Definitions
{
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

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
#if DEBUG
                WriteIndented = true,
#endif
                IgnoreNullValues = true,
            };

            options.Converters.Add(new CellWriteConverter());

            return JsonSerializer.Serialize(this, options);
        }

        public static GameDefinition FromJson(string json)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            };

            options.Converters.Add(new GameDefinitionReadConverter());
            options.Converters.Add(new TechTreeReadConverter());
            options.Converters.Add(new BattlefieldReadConverter());
            options.Converters.Add(new BuildingTypeReadConverter());
            options.Converters.Add(new UnitTypeReadConverter());

            return JsonSerializer.Deserialize<GameDefinition>(json, options);
        }
    }
}
