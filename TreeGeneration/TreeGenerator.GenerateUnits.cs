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

                    GenerateUnit(unit, role, unitValue);
                }
            }
        }

        private void GenerateUnit(UnitBuilder unit, UnitRole role, double value)
        {
            unit.AllocateName(UsedNames);

            unit.VisionRange = UnitVisionRange;

            unit.MoveRange = BaseUnitMoveRange;

            unit.BuildTime = Math.Max(1, Random.Next(value * 0.5, value * 1.25));

            unit.Health = Random.Next(value * 5, value * 8)
                .RoundNearest(5);

            unit.Armor = 0;

            switch (role)
            {
                case UnitRole.AllRounder:
                default:
                    GenerateAllRounder(unit, value);
                    break;
                case UnitRole.DamageDealer:
                    GenerateDamageDealer(unit, value);
                    break;
                case UnitRole.Scout:
                    GenerateScout(unit, value);
                    break;
                case UnitRole.MeatShield:
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

        private void GenerateAllRounder(UnitBuilder unit, double value)
        {
            // TODO: give it an attack, two minor stat boosts, and a random ability
        }

        private void GenerateDamageDealer(UnitBuilder unit, double value)
        {
            // TODO: give it a powerful attack, a damaging ability, and a minor stat reduction
        }

        private void GenerateScout(UnitBuilder unit, double value)
        {
            // TODO: give it a weak attack, a health reduction and a big speed boost, plus a utility ability
        }

        private void GenerateMeatShield(UnitBuilder unit, double value)
        {
            // TODO: give it a health boost, an armor boost, an attack, plus a random ability
        }

        private void GenerateSupportCaster(UnitBuilder unit, double value)
        {
            // TODO: give it reduced health, mana, either three support abilities or two support plus one offensive, plus either make it a detector or give it an attack.
        }

        private void GenerateOffensiveCaster(UnitBuilder unit, double value)
        {
            // TODO: give it mana, two offensive abilities plus one support or utility, and possibly give it a weak attack
        }
    }
}
