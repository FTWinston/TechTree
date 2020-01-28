using GameModels.Definitions;
using System;

namespace GameGenerator.TreeGeneration
{
    public class FeatureGenerator
    {
        public delegate Feature GenerationFunc(ref double value);

        public FeatureGenerator(GenerationFunc generate, double? min = null, double? max = null)
        {
            Generate = generate;
            MinValue = min;
            MaxValue = max;
        }

        public double? MinValue { get; }

        public double? MaxValue { get; }

        public GenerationFunc Generate { get; }
    }
}
