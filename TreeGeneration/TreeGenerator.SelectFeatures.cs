using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeGeneration
{
    public partial class TreeGenerator
    {
        public delegate Feature FeatureSelector(ref double remainingValue);

        private List<FeatureGenerator> AttackFeatures = new List<FeatureGenerator>();

        private List<FeatureGenerator> UtilityFeatures = new List<FeatureGenerator>();

        private List<FeatureGenerator> OffensiveFeatures = new List<FeatureGenerator>();

        private List<FeatureGenerator> DefensiveFeatures = new List<FeatureGenerator>();

        private void PrepareFeatureSelection()
        {
            // TODO: set up lists of features for each type, or rather feature generators.
            // Each of these can specify a min and max allowed value, and then have a function that takes a value and outputs an appropriately-scaled feature
            throw new NotImplementedException();
        }

        private Feature PickFeature(List<FeatureGenerator> options, ref double value)
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

        private static readonly UnitStat[] unitStatValues = Enum.GetValues(typeof(UnitStat))
            .OfType<UnitStat>()
            .ToArray();

        private double AdjustOneStat(UnitBuilder unit, double valueBudget)
        {
            var randomStat = unitStatValues[Random.Next(unitStatValues.Length)];

            switch (randomStat)
            {
                case UnitStat.Health:
                    return AdjustHealthStat(unit, valueBudget);

                case UnitStat.Armor:
                    return AdjustArmorStat(unit, valueBudget);

                case UnitStat.Cost:
                    return AdjustCostStat(unit, valueBudget);

                case UnitStat.BuildTime:
                    return AdjustBuildTimeStat(unit, valueBudget);
                
                case UnitStat.Supply:
                    return AdjustSupplyCostStat(unit, valueBudget);
                
                case UnitStat.MoveRange:
                    return AdjustMoveRangeStat(unit, valueBudget);

                default:
                    return 0;
            }
        }

        double ValuePerHealth = 0.01;

        private double AdjustHealthStat(UnitBuilder unit, double valueBudget)
        {
            var fullConversionGain = valueBudget / ValuePerHealth;
            var actualGain = fullConversionGain.RoundNearestDown(5);
            var actualSpend = actualGain * ValuePerHealth;

            unit.Health += actualGain;

            return actualSpend;
        }

        double ValuePerArmor = 0.2;

        private double AdjustArmorStat(UnitBuilder unit, double valueBudget)
        {
            var fullConversionGain = valueBudget / ValuePerArmor;
            var actualGain = fullConversionGain.RoundNearestDown(1);
            var actualSpend = actualGain * ValuePerArmor;

            unit.Armor += actualGain;

            return actualSpend;
        }

        double ValuePerCostMultiplierIncrement = 0.1;

        private double AdjustCostStat(UnitBuilder unit, double valueBudget)
        {
            var fullConversionGain = valueBudget / ValuePerCostMultiplierIncrement;
            var actualGain = fullConversionGain.RoundNearestDown(1);
            var actualSpend = actualGain * ValuePerCostMultiplierIncrement;

            foreach (var cost in unit.Cost)
                if (cost.Key != ResourceType.Supply)
                    unit.Cost[cost.Key] = cost.Value * -actualGain;

            return actualSpend;
        }

        double ValuePerBuildTimeReduction = 0.3;

        private double AdjustBuildTimeStat(UnitBuilder unit, double valueBudget)
        {
            var fullConversionGain = valueBudget / ValuePerBuildTimeReduction;
            var actualGain = fullConversionGain.RoundNearestDown(1);
            
            if (actualGain >= unit.BuildTime)
                actualGain = unit.BuildTime - 1;

            if (actualGain <= 0)
                return 0;

            var actualSpend = actualGain * ValuePerBuildTimeReduction;

            unit.BuildTime -= actualGain;

            return actualSpend;
        }

        double ValuePerSupplyCostReduction = 0.2;

        private double AdjustSupplyCostStat(UnitBuilder unit, double valueBudget)
        {
            var fullConversionGain = valueBudget / ValuePerSupplyCostReduction;
            var actualGain = fullConversionGain.RoundNearestDown(1);

            if (actualGain >= unit.Cost[ResourceType.Supply])
                actualGain = unit.Cost[ResourceType.Supply] - 1;

            if (actualGain <= 0)
                return 0;

            var actualSpend = actualGain * ValuePerSupplyCostReduction;

            unit.Cost[ResourceType.Supply] -= actualGain;

            return actualSpend;
        }

        double ValuePerMoveRange = 0.4;

        private double AdjustMoveRangeStat(UnitBuilder unit, double valueBudget)
        {
            var fullConversionGain = valueBudget / ValuePerMoveRange;
            var actualGain = fullConversionGain.RoundNearestDown(1);

            var actualSpend = actualGain * ValuePerMoveRange;

            unit.MoveRange += actualGain;

            return actualSpend;
        }

        private void AdjustForRemainingValue(UnitBuilder unit, double remainingValue)
        {
            if (remainingValue > 0)
            {
                // spend this any way we can ... slightly reduced cost or HP, most likely
                throw new NotImplementedException();
            }
            else if (remainingValue < 0)
            {
                // claw back this overspend by increasing cost or reducing HP
                throw new NotImplementedException();
            }
        }

        private Feature SelectAttack(ref double valueBudget)
        {
            return PickFeature(AttackFeatures, ref valueBudget);
        }

        private Feature SelectUtility(ref double valueBudget)
        {
            return PickFeature(UtilityFeatures, ref valueBudget);
        }

        private Feature SelectOffensiveAbility(ref double valueBudget)
        {
            return PickFeature(OffensiveFeatures, ref valueBudget);
        }

        private Feature SelectDefensiveAbility(ref double valueBudget)
        {
            return PickFeature(DefensiveFeatures, ref valueBudget);
        }
    }
}
