using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameGenerator.TreeGeneration
{
    public partial class TreeGenerator
    {
        public delegate Feature? FeatureSelector(ref double remainingValue);

        private List<FeatureGenerator> AttackFeatures = new List<FeatureGenerator>();

        private List<FeatureGenerator> UtilityFeatures = new List<FeatureGenerator>();

        private List<FeatureGenerator> OffensiveFeatures = new List<FeatureGenerator>();

        private List<FeatureGenerator> DefensiveFeatures = new List<FeatureGenerator>();

        private void PrepareFeatureSelection()
        {
            // TODO: set up lists of features for each type, or rather feature generators.
            // Each of these can specify a min and max allowed value, and then have a function that takes a value and outputs an appropriately-scaled feature
            //throw new NotImplementedException();
        }

        private Feature? PickFeature(List<FeatureGenerator> options, ref double value)
        {
            var budget = value;

            var generator = options
                .Where(o => !o.MinValue.HasValue || o.MinValue <= budget)
                .Where(o => !o.MaxValue.HasValue || o.MaxValue >= budget)
                .FirstOrDefault();

            if (generator == null)
                return null;

            var feature = generator.Generate(ref value);

            options.Remove(generator);

            return feature;
        }

        private double AddFeature(UnitBuilder unit, FeatureSelector selectFeature, double valueBudget)
        {
            var feature = selectFeature(ref valueBudget);

            if (feature == null)
                return 0;

            unit.Features.Add(feature);
            return valueBudget;
        }

        private Feature? SelectAttack(ref double valueBudget)
        {
            return PickFeature(AttackFeatures, ref valueBudget);
        }

        private Feature? SelectUtility(ref double valueBudget)
        {
            return PickFeature(UtilityFeatures, ref valueBudget);
        }

        private Feature? SelectOffensiveAbility(ref double valueBudget)
        {
            return PickFeature(OffensiveFeatures, ref valueBudget);
        }

        private Feature? SelectDefensiveAbility(ref double valueBudget)
        {
            return PickFeature(DefensiveFeatures, ref valueBudget);
        }
    }
}
