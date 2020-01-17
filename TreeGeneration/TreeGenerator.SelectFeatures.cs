using GameModels.Definitions;
using System;
using System.Linq;

namespace TreeGeneration
{
    public partial class TreeGenerator
    {
        public delegate Feature FeatureSelector(ref double remainingValue);

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

        private double AdjustHealthStat(UnitBuilder unit, double valueBudget)
        {
            throw new NotImplementedException();
        }

        private double AdjustArmorStat(UnitBuilder unit, double valueBudget)
        {
            throw new NotImplementedException();
        }

        private double AdjustCostStat(UnitBuilder unit, double valueBudget)
        {
            throw new NotImplementedException();
        }

        private double AdjustBuildTimeStat(UnitBuilder unit, double valueBudget)
        {
            throw new NotImplementedException();
        }

        private double AdjustSupplyCostStat(UnitBuilder unit, double valueBudget)
        {
            throw new NotImplementedException();
        }

        private double AdjustMoveRangeStat(UnitBuilder unit, double valueBudget)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private Feature SelectUtility(ref double valueBudget)
        {
            throw new NotImplementedException();
        }

        private Feature SelectOffensiveAbility(ref double valueBudget)
        {
            throw new NotImplementedException();
        }

        private Feature SelectDefensiveAbility(ref double valueBudget)
        {
            throw new NotImplementedException();
        }
    }
}
