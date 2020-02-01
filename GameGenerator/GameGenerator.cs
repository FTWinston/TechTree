using GameModels.Definitions;
using GameGenerator.BattlefieldGeneration;
using GameGenerator.TreeGeneration;
using System;

namespace GameGenerator
{
    public static class GameGenerator
    {
        public static GameDefinition GenerateGame()
        {
            int seed = new Random().Next();
            return GenerateGame(seed);
        }

        public static GameDefinition GenerateGame(int seed)
        {
            Random random = new Random(seed);

            // TODO: how is tree complexity and battlefield size controlled, if it is at all?

            var techTree = new TreeGenerator(random.Next())
                .Generate();

            // TODO: should more or less water affect the prevalence of aquatic units?

            var battlefield = new BattlefieldGenerator(random.Next())
                .Generate();

            int complexity = 1;

            return new GameDefinition(seed, complexity, techTree, battlefield);
        }
    }
}
