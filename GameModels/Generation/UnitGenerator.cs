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
                UnitRole = UnitType.Role.AllRounder,
            };

            return unit;
        }

        public static UnitType GenerateWorker(TreeGenerator gen)
        {
            string symbol = TreeGenerator.workerSymbol.ToString();
            Random r = gen.Random;

            UnitType unit = new UnitType()
            {
                Name = "Worker " + symbol,
                Symbol = symbol,
            };

            UnitGenerator.Populate(gen, unit, UnitType.Role.Worker, 1);

            return unit;
        }

        public static void Populate(TreeGenerator gen, UnitType unit, UnitType.Role function, int tier)
        {
            SetupBaseStats(gen.Random, unit, tier);
            unit.UnitRole = function;

            while (true)
            {
                switch (function)
                {
                    case UnitType.Role.AllRounder:
                        PopulateAllRounder(gen, unit, tier);
                        break;
                    case UnitType.Role.DamageDealer:
                        PopulateDamageDealer(gen, unit, tier);
                        break;
                    case UnitType.Role.MeatShield:
                        PopulateMeatShield(gen, unit, tier);
                        break;
                    case UnitType.Role.SupportCaster:
                        PopulateSupportCaster(gen, unit, tier);
                        break;
                    case UnitType.Role.OffensiveCaster:
                        PopulateOffensiveCaster(gen, unit, tier);
                        break;
                    case UnitType.Role.Scout:
                        PopulateScout(gen, unit, tier);
                        break;
                    case UnitType.Role.Infiltrator:
                        PopulateInfiltrator(gen, unit, tier);
                        break;
                    case UnitType.Role.Transport:
                        PopulateTransport(gen, unit, tier);
                        break;
                    case UnitType.Role.Worker:
                        PopulateWorker(gen, unit, tier);
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

        private static void SetupBaseStats(Random r, UnitType unit, int tier)
        {
            unit.VisionRange = 3;
            unit.ActionPoints = 4;
            unit.Tier = tier;
            unit.BuildTime = r.Next(Math.Max(1, tier - 2), tier + 2);


            // 10-15, plus 13-20 per tier
            unit.Health = r.Next(10, 16) + r.Next(13, 21) * unit.Tier;

            // 30% chance of 1, plus 0.5 - 0.8 per tier, rounded down
            unit.Armor = (r.Next(10) < 3 ? 1 : 0) + (int)(r.Next(5, 9) * unit.Tier / 10f);

            // 5-10 plus 10 - 20 per tier
            unit.Mana = r.Next(5, 11) + r.Next(10, 21) * unit.Tier;
            

            unit.MineralCost = r.Next(35, 61) + r.Next(20, 31) * unit.Tier;
            unit.VespineCost = Math.Max(0, 25 * (unit.Tier - 1));
            unit.SupplyCost = unit.Tier;
        }

        private static void PopulateAllRounder(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(1.1);
            unit.Mana = 0;

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.05);

            // TODO: abilities
            var features = Feature.GetAttackFeatures();
            AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());
        }

        private static void PopulateDamageDealer(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.9);
            unit.Armor = unit.Armor.Scale(0.75);
            unit.Mana = unit.Mana.Scale(0.75);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.05);
            unit.VespineCost = unit.VespineCost.Scale(1.25);
            unit.SupplyCost = unit.SupplyCost.Scale(1.1);

            // TODO: abilities
            var features = Feature.GetAttackFeatures();
            AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());
        }

        private static void PopulateMeatShield(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(1.7);
            unit.Armor = unit.Armor.Scale(2.25);
            unit.Mana = unit.Mana.Scale(0.55);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.2);
            unit.VespineCost = unit.VespineCost.Scale(1.1);
            unit.SupplyCost = unit.SupplyCost.Scale(1.25);

            // TODO: abilities

            // add an attack feature
            var features = Feature.GetAttackFeatures();
            AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());

            features = Feature.GetPassiveFeatures();

            // 0-2 passive features
            for (int i=gen.Random.Next(3); i>=0; i--)
                AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());
        }

        private static void PopulateSupportCaster(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.55);
            unit.Armor = 0;

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.5);
            unit.VespineCost = unit.VespineCost.Scale(2) + 25;
            unit.SupplyCost = unit.SupplyCost.Scale(0.75);

            // TODO: abilities
        }

        private static void PopulateOffensiveCaster(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.7);
            unit.Armor = unit.Armor.Scale(0.3);
            unit.Mana = unit.Mana.Scale(1.15);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.55);
            unit.VespineCost = unit.VespineCost.Scale(2.25) + 25;
            unit.SupplyCost = unit.SupplyCost.Scale(0.9);

            // TODO: abilities
        }

        private static void PopulateScout(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.7);
            unit.Armor = unit.Armor.Scale(0.45);
            unit.Mana = unit.Mana.Scale(0.65);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.6);
            unit.VespineCost = unit.VespineCost.Scale(0.7);
            unit.SupplyCost = unit.SupplyCost.Scale(0.7);

            // TODO: abilities
            var features = Feature.GetAttackFeatures();
            AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());
        }

        private static void PopulateInfiltrator(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.85);
            unit.Armor = unit.Armor.Scale(0.7);
            unit.Mana = unit.Mana.Scale(0.8);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.2);
            unit.VespineCost = unit.VespineCost.Scale(1.6);
            unit.SupplyCost = unit.SupplyCost.Scale(1.4);

            // TODO: abilities
            var features = Feature.GetAttackFeatures();
            AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());

            features = Feature.GetPassiveFeatures();

            // 0-1 passive features
            for (int i = gen.Random.Next(3); i >= 0; i--)
                AddFeature(gen.Random, unit, PickRandomIndex(gen.Random, features)());

            // TODO: add spell feature(s)!
        }

        private static void PopulateTransport(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(1.05);
            unit.Armor = unit.Armor.Scale(1.25);
            unit.Mana = unit.Mana.Scale(0.5);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.65);
            unit.VespineCost = unit.VespineCost.Scale(0.25);
            unit.SupplyCost = unit.SupplyCost.Scale(0.45);

            // TODO: abilities
        }

        private static void PopulateWorker(TreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.75);
            unit.Armor = 0;
            unit.Mana = 0;

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.55);
            unit.VespineCost = 0;
            unit.SupplyCost = 1;

            // TODO: abilities
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
