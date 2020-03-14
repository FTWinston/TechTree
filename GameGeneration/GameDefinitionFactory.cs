using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameGeneration.TreeGeneration;
using System;
using ObjectiveStrategy.GameModels;

namespace ObjectiveStrategy.GameGeneration
{
    public class GameDefinitionFactory
    {
        internal const int MaxComplexity = 100;

        public GameDefinition GenerateGame()
        {
            var random = new Random();
            int complexity = random.Next(1, MaxComplexity + 1);
            int seed = random.Next();

            return GenerateGame(complexity, seed);
        }

        public GameDefinition GenerateGame(int complexity)
        {
            int seed = new Random().Next();
            return GenerateGame(complexity, seed);
        }

        public GameDefinition GenerateGame(int complexity, int seed)
        {
            if (complexity < 1 || complexity > MaxComplexity)
                throw new ArgumentException($"Invalid complexity value; expected value in range 1 to {MaxComplexity}");

            Random random = new Random(seed);

            int turnLimit = complexity; // TODO: this is now how turn limit should be calculated

            var techTree = new TreeGenerator(complexity, random.Next())
                .Generate();

            // TODO: should more or less water affect the prevalence of aquatic units?

            var battlefield = new BattlefieldGenerator(complexity, random.Next())
                .Generate();

            var objectives = new Objective[] { }; // TODO: actually generate these

            return new GameDefinition
            (
                complexity: complexity,
                seed: seed,
                turnLimit: turnLimit,
                techTree: techTree,
                battlefield: battlefield,
                objectives: objectives
            );
        }
    }
}
