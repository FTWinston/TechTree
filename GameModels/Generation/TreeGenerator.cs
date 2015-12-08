using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Generation
{
    public class TreeGenerator
    {
        public static TechTree Generate(Complexity complexity = Complexity.Normal, int? seed = null)
        {
            Random r = seed == null ? new Random() : new Random(seed.Value);

            TechTree tree = new TechTree();

            int numFactories;
            switch (complexity)
            {
                case Complexity.Simple:
                    numFactories = 2;
                    break;
                case Complexity.Normal:
                    numFactories = 3;
                    break;
                case Complexity.Complex:
                    numFactories = 4;
                    break;
                default:
                    throw new ArgumentException("Unexpected Complexity value: " + complexity);
            }

            for (int i=0; i<numFactories; i++)
            {
                tree.Buildings.Add(BuildingGenerator.GenerateFactory(r, i));
            }

            UnitType unit;
            do
            {
                unit = UnitGenerator.Generate(r, UnitType.Role.Fighter, 1);
            } while (unit == null);
            tree.Units.Add(unit);

            do
            {
                unit = UnitGenerator.Generate(r, UnitType.Role.Hybrid, 1);
            } while (unit == null);
            tree.Units.Add(unit);

            do
            {
                unit = UnitGenerator.Generate(r, UnitType.Role.Fighter, 2);
            } while (unit == null);
            tree.Units.Add(unit);

            do
            {
                unit = UnitGenerator.Generate(r, UnitType.Role.Hybrid, 2);
            } while (unit == null);
            tree.Units.Add(unit);

            do
            {
                unit = UnitGenerator.Generate(r, UnitType.Role.Caster, 2);
            } while (unit == null);
            tree.Units.Add(unit);

            return tree;
        }

        public enum Complexity
        {
            Simple = 1,
            Normal = 2,
            Complex = 3,
        }
    }
}
