using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameGenerator.TreeGeneration
{
    public partial class TreeGenerator
    {
        private static readonly UnitStat[] unitStatValues = Enum.GetValues(typeof(UnitStat))
            .OfType<UnitStat>()
            .ToArray();

        private double AdjustStats(UnitBuilder unit, double valueBudget)
        {
            var remainingBudget = valueBudget;
            var statsInOrder = unitStatValues.ToRandomQueue(Random);

            var allocatedAny = false;
            while (allocatedAny)
            {
                allocatedAny = false;

                foreach (var stat in statsInOrder)
                {
                    var statBudget = Math.Abs(remainingBudget) > 0.1
                        ? remainingBudget * Random.NextDouble(0.25, 0.65)
                        : remainingBudget;

                    var adjustment = AdjustStat(unit, statBudget, stat);
                    if (adjustment != 0)
                        allocatedAny = true;

                    remainingBudget -= adjustment;
                }
            }

            return valueBudget - remainingBudget;
        }

        private double AdjustRandomStat(UnitBuilder unit, double valueBudget)
        {
            var randomStat = unitStatValues[Random.Next(unitStatValues.Length)];

            return AdjustStat(unit, valueBudget, randomStat);
        }

        private double AdjustStat(UnitBuilder unit, double valueBudget, UnitStat stat)
        {
            switch (stat)
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

            foreach (var cost in unit.Cost.ToArray())
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
    }
}
