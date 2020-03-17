using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameGeneration
{
    public class ObjectiveGenerator
    {
        public ObjectiveGenerator(int complexity, int seed)
        {
            Complexity = complexity;
            Seed = seed;
        }

        private Random Random { get; set; } = new Random();

        public int Complexity { get; }

        public int Seed { get; }

        public Objective[] Generate()
        {
            Random = new Random(Seed);

            var results = new List<Objective>();

            // TODO: generate a proper set of these

            results.Add(new Objective
            {
                CellsByPlayer = new Dictionary<uint, int[]>
                {
                    { 0, new int[] { 4, 5, 6 } },
                    { 1, new int[] { 1, 2, 3 } },
                    { 2, new int[] { 7, 8, 9 } },
                },
                Value = 5,
                Description = "Test objective, no real meaning, just setting all the fields",
                Subject = ObjectiveSubject.Units,
                // FeatureTypeID = 1,
                RelativeToOpponent = true,
                TargetQuantity = 10,
            });

            return results.ToArray();
        }
    }
}
