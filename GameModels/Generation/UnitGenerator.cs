using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Generation
{
    static class UnitGenerator
    {
        public static UnitType GenerateStub(TreeGenerator gen)
        {
            string symbol = gen.GetUnusedSymbol();
            UnitType unit = new UnitType()
            {
                Name = "Unit " + symbol,
                Symbol = symbol,
            };

            return unit;
        }

        public static void Populate(TreeGenerator gen, UnitType unit, UnitType.Role function, int tier)
        {
            Random r = gen.Random;

            unit.UnitRole = function;
            unit.VisionRange = 3;
            unit.ActionPoints = 4;
            unit.Tier = tier;
            unit.BuildTime = r.Next(Math.Max(1, tier - 2), tier + 2);

            while (true)
            {
                switch (function)
                {
                    case UnitType.Role.AllRounder:
                    case UnitType.Role.DamageDealer:
                    case UnitType.Role.MeatShield:
                        PopulateFighter(r, unit);
                        break;

                    case UnitType.Role.SupportCaster:
                    case UnitType.Role.OffenseCaster:
                    case UnitType.Role.Worker:
                        PopulateCaster(r, unit);
                        break;

                    case UnitType.Role.Scout:
                    case UnitType.Role.Infiltrator:
                    case UnitType.Role.Transport:
                        PopulateHybrid(r, unit);
                        break;
                    default:
                        throw new Exception("Unexpected unit role: " + function);
                }

                // check that the allocated features fit well together
                bool featuresValid = true;
                foreach (var feature in unit.Features)
                    if (!feature.Validate(unit))
                    {
                        featuresValid = false;
                        break;
                    }

                // check units with mana have features that use mana, and vice-versa
                if (unit.Mana > 0)
                {
                    bool usesMana = false;
                    foreach (var feature in unit.Features)
                        if (feature.UsesMana)
                        {
                            usesMana = true;
                            break;
                        }

                    if (!usesMana)
                    {
                        featuresValid = false;
                        break;
                    }
                }
                else
                    foreach (var feature in unit.Features)
                        if (feature.UsesMana)
                        {
                            featuresValid = false;
                            break;
                        }

                if (featuresValid)
                    break;
            }

            // apply a little rounding, to make things feel less artificial
            unit.Health = unit.Health.RoundNearest(5);
            unit.Mana = unit.Mana.RoundNearest(5);

            unit.MineralCost = unit.MineralCost.RoundNearest(5);
            unit.VespineCost = unit.VespineCost.RoundNearest(5);
        }

        private static void PopulateFighter(Random r, UnitType unit)
        {
            // 8-15 plus 15 - 30 per tier
            unit.Health = r.Next(8, 16) + r.Next(15, 31) * unit.Tier;

            // 30% chance of 1, plus 0.5 - 1 per tier, rounded down
            unit.Armor = (r.Next(10) < 3 ? 1 : 0) + (int)(r.Next(5, 11) * unit.Tier / 10f);

            // no mana for fighters
            unit.Mana = 0;

            // TODO: calculate cost based on actual stats, not just tier
            unit.MineralCost = Math.Max(0, 50 * unit.Tier);
            unit.VespineCost = Math.Max(0, 25 * (unit.Tier - 1));
            unit.SupplyCost = unit.Tier;

            // add an attack feature
            var features = Feature.GetAttackFeatures();
            AddFeature(r, unit, PickRandomIndex(r, features)());

            features = Feature.GetPassiveFeatures();

            // 0-2 passive features
            for (int i=r.Next(3); i>=0; i--)
                AddFeature(r, unit, PickRandomIndex(r, features)());
        }

        private static void PopulateCaster(Random r, UnitType unit)
        {
            // 5-10 plus 5 - 15 per tier
            unit.Health = r.Next(5, 11) + r.Next(5, 16) * unit.Tier;

            // 0.2 - 0.5 per tier, rounded down
            unit.Armor = (int)(r.Next(2, 6) * unit.Tier / 10f);

            // 5-10 plus 10 - 20 per tier
            unit.Mana = r.Next(5, 11) + r.Next(10, 21) * unit.Tier;

            // TODO: calculate cost based on actual stats, not just tier
            unit.MineralCost = Math.Max(0, 25 + 25 * unit.Tier);
            unit.VespineCost = Math.Max(0, 50 * unit.Tier);
            unit.SupplyCost = unit.Tier;

            // TODO: possibly add an attack feature!

            // TODO: add spell features!
        }

        private static void PopulateHybrid(Random r, UnitType unit)
        {
            // 5-12 plus 10 - 20 per tier
            unit.Health = r.Next(5, 13) + r.Next(10, 21) * unit.Tier;

            // 20% chance of 1, plus 0.3 - 0.8 per tier, rounded down
            unit.Armor = (r.Next(10) < 2 ? 1 : 0) + (int)(r.Next(3, 9) * unit.Tier / 10f);

            // 5-10 plus 5 - 15 per tier
            unit.Mana = r.Next(5, 11) + r.Next(5, 16) * unit.Tier;

            // TODO: calculate cost based on actual stats, not just tier
            unit.MineralCost = Math.Max(0, 40 * unit.Tier);
            unit.VespineCost = Math.Max(0, 25 * unit.Tier);
            unit.SupplyCost = unit.Tier;

            // add an attack feature
            var features = Feature.GetAttackFeatures();
            AddFeature(r, unit, PickRandomIndex(r, features)());

            features = Feature.GetPassiveFeatures();

            // 0-1 passive features
            for (int i = r.Next(3); i >= 0; i--)
                AddFeature(r, unit, PickRandomIndex(r, features)());

            // TODO: add spell feature(s)!
        }

        private static void AddFeature(Random r, UnitType unit, Feature feature)
        {
            unit.Features.Add(feature);
            double costScale = feature.Initialize(unit, r);

            unit.MineralCost = (int)(unit.MineralCost * costScale);
            unit.VespineCost = (int)(unit.VespineCost * costScale);
        }

        private static T PickRandomIndex<T>(Random r, IList<T> options)
        {
            return options[r.Next(options.Count)];
        }
    }
}
