using GameModels.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModels.Definitions.Builders
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

        protected void GenerateUnits()
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
                    GenerateUnit(unit, role);
                }
            }
        }

        private int DetermineTier(UnitBuilder unit)
        {
            int tier = 0;
            var building = Buildings[unit.Prerequisite ?? unit.BuiltBy];

            while (true)
            {
                tier++;

                if (!building.Prerequisite.HasValue)
                    return tier;

                building = Buildings[building.Prerequisite.Value];
            }
        }

        private void GenerateUnit(UnitBuilder unit, UnitRole role)
        {
            int tier = DetermineTier(unit);
            unit.Tier = tier;
            unit.VisionRange = UnitVisionRange;
            unit.AllocateName(UsedNames);

            // TODO: implement this for each unit role
        }
    }
}
