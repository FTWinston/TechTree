using GameModels.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeGeneration
{
    public partial class TreeGenerator
    {
        private int nextUnitSymbol = 0;

        private string AllocateUnitSymbol()
        {
            return nextUnitSymbol >= unitSymbols.Length
                ? "-"
                : unitSymbols[nextUnitSymbol++].ToString();
        }

        protected void CreateUnits()
        {
            int numUnits = Random.Next((int)Complexity * 3, (int)Complexity * 4);
            for (int i = 0; i < numUnits; i++)
            {
                var symbol = AllocateUnitSymbol();
                Units.Add(nextIdentifier++, new UnitBuilder(Random, symbol));
            }
        }

        protected void GenerateUnits(List<UnitBuilder> unitsTreeOrder)
        {
            // get a queue of all units, in a random order
            var units = Units.Values
                .ToArray()
                .ToRandomQueue(Random);

            var allRoles = Enum.GetValues(typeof(UnitRole))
                .Cast<UnitRole>()
                .ToList();

            allRoles.Randomize(Random);

            while (true)
            {
                // Populate every role once, as long as we have units to populate.
                foreach (var role in allRoles)
                {
                    if (units.Count == 0)
                        return;

                    UnitBuilder unit = units.Dequeue();

                    var techProgressionFraction = (float)unitsTreeOrder.IndexOf(unit) / unitsTreeOrder.Count;
                    var unitValue = BaseUnitValue + (MaxUnitValue - BaseUnitValue) * techProgressionFraction;

                    GenerateUnit(unit, role, unitValue, techProgressionFraction);
                }
            }
        }

        private void GenerateUnit(UnitBuilder unit, UnitRole role, double value, double techProgressionFraction)
        {
            unit.AllocateName(UsedNames);

            unit.VisionRange = UnitVisionRange;

            unit.MoveRange = BaseUnitMoveRange;

            // standard health, armor, mana, cost, supply cost and build time values for its value

            unit.BuildTime = Math.Max(1, Random.Next(value * 0.75, value * 0.95));

            var overallCost = (int)value * Random.Next(25, 40);
            unit.Cost = SplitResourceCosts(overallCost, techProgressionFraction);

            unit.Cost[GameModels.Definitions.ResourceType.Supply] = Math.Max(1, Random.Next(value * 0.75, value * 0.95));

            unit.Health = Random.Next(value * 20, value * 35)
                .RoundNearest(5);

            unit.Mana = Random.Next(value * 40, value * 60)
                .RoundNearest(5);

            unit.Armor = Random.Next(value * 0.25, value * 0.9);

            switch (role)
            {
                case UnitRole.AllRounder:
                default:
                    unit.Mana = 0;
                    GenerateAllRounder(unit, value);
                    break;
                case UnitRole.DamageDealer:
                    unit.Mana = 0;
                    GenerateDamageDealer(unit, value);
                    break;
                case UnitRole.Scout:
                    unit.Mana = 0;
                    GenerateScout(unit, value);
                    break;
                case UnitRole.MeatShield:
                    unit.Mana = 0;
                    GenerateMeatShield(unit, value);
                    break;
                case UnitRole.SupportCaster:
                    GenerateSupportCaster(unit, value);
                    break;
                case UnitRole.OffensiveCaster:
                    GenerateOffensiveCaster(unit, value);
                    break;
            }
        }

        private void GenerateAllRounder(UnitBuilder unit, double remainingValue)
        {
            // Spend roughly half its value on an attack
            remainingValue -= AddFeature(unit, SelectAttack, remainingValue * Random.NextDouble(0.35, 0.55));

            // Spend roughly half of the remainder on a utility ability.
            remainingValue -= AddFeature(unit, SelectUtility, remainingValue * Random.NextDouble(0.25, 0.65));

            // Spend the remainder on 2-3 stat boosts ... health, armor, speed, reduced cost, reduced build time
            remainingValue -= AdjustOneStat(unit, remainingValue * Random.NextDouble(0.3, 0.6));

            if (Random.Next(3) == 0)
                remainingValue -= AdjustOneStat(unit, remainingValue * Random.NextDouble(0.3, 0.6));

            remainingValue -= AdjustOneStat(unit, remainingValue);

            AdjustForRemainingValue(unit, remainingValue);
        }

        private void GenerateDamageDealer(UnitBuilder unit, double remainingValue)
        {
            // spend most of its value on an attack
            remainingValue -= AddFeature(unit, SelectAttack, remainingValue * Random.NextDouble(0.65, 0.85));

            // buy back value by reducing health
            remainingValue -= AdjustHealthStat(unit, -remainingValue * Random.NextDouble(0.7, 1.5));

            // spend remaining value on an offensive ability, or a utility (less likely)
            if (Random.Next(4) == 0)
                remainingValue -= AddFeature(unit, SelectUtility, remainingValue);
            else
                remainingValue -= AddFeature(unit, SelectOffensiveAbility, remainingValue);

            // any leftover value can go on a stat boost
            remainingValue -= AdjustOneStat(unit, remainingValue);

            AdjustForRemainingValue(unit, remainingValue);
        }

        private void GenerateScout(UnitBuilder unit, double remainingValue)
        {
            // give it a fairly weak attack
            remainingValue -= AddFeature(unit, SelectAttack, remainingValue * Random.NextDouble(0.15, 0.25));

            // buy back value by reducing health
            remainingValue -= AdjustHealthStat(unit, -remainingValue * Random.NextDouble(0.1, 0.35));

            // a big speed boost
            remainingValue -= AdjustMoveRangeStat(unit, remainingValue * Random.NextDouble(0.35, 0.7));

            // and a utility ability
            remainingValue -= AddFeature(unit, SelectUtility, remainingValue * Random.NextDouble(0.75, 1));

            // any leftover value can go on a stat boost
            remainingValue -= AdjustOneStat(unit, remainingValue);

            AdjustForRemainingValue(unit, remainingValue);
        }

        private void GenerateMeatShield(UnitBuilder unit, double remainingValue)
        {
            // give it an attack
            remainingValue -= AddFeature(unit, SelectAttack, remainingValue * Random.NextDouble(0.2, 0.45));

            // boost its health and armor
            remainingValue -= AdjustHealthStat(unit, remainingValue * Random.NextDouble(0.35, 0.65));
            remainingValue -= AdjustArmorStat(unit, remainingValue * Random.NextDouble(0.4, 0.6));

            // plus a random ability
            if (Random.Next(2) == 0)
                remainingValue -= AddFeature(unit, SelectUtility, remainingValue);
            else
                remainingValue -= AddFeature(unit, SelectOffensiveAbility, remainingValue);

            // any leftover value can go on a stat boost
            remainingValue -= AdjustOneStat(unit, remainingValue);

            AdjustForRemainingValue(unit, remainingValue);
        }

        private void GenerateSupportCaster(UnitBuilder unit, double remainingValue)
        {
            // buy back value by reducing health
            remainingValue -= AdjustHealthStat(unit, -remainingValue * Random.NextDouble(0.15, 0.55));

            // either three support abilities or two support plus one offensive
            remainingValue -= AddFeature(unit, SelectDefensiveAbility, remainingValue * Random.NextDouble(0.2, 0.5));

            remainingValue -= AddFeature(unit, SelectDefensiveAbility, remainingValue * Random.NextDouble(0.3, 0.7));

            if (Random.Next(2) == 0)
                remainingValue -= AddFeature(unit, SelectOffensiveAbility, remainingValue * Random.NextDouble(0.5, 0.8));
            else
                remainingValue -= AddFeature(unit, SelectDefensiveAbility, remainingValue * Random.NextDouble(0.5, 0.8));

            // plus either make it a detector or give it an attack.
            if (Random.Next(2) == 0)
                unit.IsDetector = true;
            else
                remainingValue -= AddFeature(unit, SelectAttack, remainingValue * Random.NextDouble(0.75, 1));

            // any leftover value can go on a stat boost
            remainingValue -= AdjustOneStat(unit, remainingValue);

            AdjustForRemainingValue(unit, remainingValue);
        }

        private void GenerateOffensiveCaster(UnitBuilder unit, double remainingValue)
        {
            // two offensive abilities plus one support or utility
            remainingValue -= AddFeature(unit, SelectOffensiveAbility, remainingValue * Random.NextDouble(0.2, 0.5));

            remainingValue -= AddFeature(unit, SelectOffensiveAbility, remainingValue * Random.NextDouble(0.3, 0.7));

            if (Random.Next(2) == 0)
                remainingValue -= AddFeature(unit, SelectUtility, remainingValue * Random.NextDouble(0.7, 0.9));
            else
                remainingValue -= AddFeature(unit, SelectDefensiveAbility, remainingValue * Random.NextDouble(0.7, 0.9));

            // and possibly give it a weak attack
            if (Random.Next(3) == 0)
                remainingValue -= AddFeature(unit, SelectAttack, remainingValue * Random.NextDouble(0.85, 1));

            // any leftover value can go on a stat boost
            remainingValue -= AdjustOneStat(unit, remainingValue);

            AdjustForRemainingValue(unit, remainingValue);
        }
    }
}
