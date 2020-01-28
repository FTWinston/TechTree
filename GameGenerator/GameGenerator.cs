using GameModels;
using GameGenerator.BattlefieldGeneration;
using GameGenerator.TreeGeneration;
using System;

namespace GameGenerator
{
    public static class GameGenerator
    {
        public static GameDefinition GenerateGame(int? seed = null)
        {
            Random random = seed.HasValue
                ? new Random(seed.Value)
                : new Random();

            // TODO: how is tree complexity and battlefield size controlled, if it is at all?

            var techTree = new TreeGenerator(random.Next())
                .Generate();

            // TODO: should more or less water affect the prevalence of aquatic units?

            var battlefield = new BattlefieldGenerator(random.Next())
                .Generate();

            return new GameDefinition(techTree, battlefield);
        }
    }
}
