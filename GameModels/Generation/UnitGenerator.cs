using GameModels.Definitions;
using GameModels.Definitions.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModels.Generation
{
    static class UnitGenerator
    {
        /*
        public static UnitType GenerateStub(OldTreeGenerator gen)
        {
            string symbol = gen.AllocateUnitSymbol();
            UnitType unit = new UnitType()
            {
                Name = "Unit " + symbol,
                Symbol = symbol,
            };

            return unit;
        }

        public static UnitType GenerateWorker(OldTreeGenerator gen)
        {   
            string symbol = gen.AllocateUnitSymbol();
            UnitType unit = new UnitType()
            {
                Name = "Worker " + symbol,
                Symbol = symbol,
            };

            UnitGenerator.Populate(gen, unit, Role.Worker, 1);

            return unit;
        }

        public static void Populate(OldTreeGenerator gen, UnitType unit, Role function, int tier)
        {
            // TODO: in the event that a type fails validation, we need to clear it down back to being a stub
            // ... and also undo the stats & flag changes that generation has implemented! ack!

            while (true)
            {
                SetupBaseStats(gen, unit, tier);

                switch (function)
                {
                    case Role.AllRounder:
                        PopulateAllRounder(gen, unit, tier);
                        break;
                    case Role.DamageDealer:
                        PopulateDamageDealer(gen, unit, tier);
                        break;
                    case Role.MeatShield:
                        PopulateMeatShield(gen, unit, tier);
                        break;
                    case Role.SupportCaster:
                        PopulateSupportCaster(gen, unit, tier);
                        break;
                    case Role.OffensiveCaster:
                        PopulateOffensiveCaster(gen, unit, tier);
                        break;
                    case Role.Scout:
                        PopulateScout(gen, unit, tier);
                        break;
                    case Role.Infiltrator:
                        PopulateInfiltrator(gen, unit, tier);
                        break;
                    case Role.Transport:
                        PopulateTransport(gen, unit, tier);
                        break;
                    case Role.Worker:
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
                else
                    unit.RemoveAllFeatures();
            }

            // apply a little rounding, to make things feel less artificial
            unit.Health = unit.Health.RoundNearest(5);
            unit.Mana = unit.Mana.RoundNearest(5);

            unit.MineralCost = unit.MineralCost.RoundNearest(5);
            unit.GasCost = unit.GasCost.RoundNearest(5);

            // flag that this unit type has been populated, so that we don't try that again later
            unit.Populated = true;
        }

        private static void SetupBaseStats(OldTreeGenerator gen, UnitType unit, int tier)
        {
            unit.VisionRange = gen.UnitVisionRange;
            unit.MoveRange = 3;
            unit.BuildTime = gen.Random.Next(Math.Max(1, tier - 2), tier + 2);
            unit.Flags = UnitFlags.None;
            unit.IsDetector = false;


            // 10-15, plus 13-20 per tier
            unit.Health = gen.Random.Next(10, 16) + gen.Random.Next(13, 21) * tier;

            // 30% chance of 1, plus 0.5 - 0.8 per tier, rounded down
            unit.Armor = (gen.Random.Next(10) < 3 ? 1 : 0) + (int)(gen.Random.Next(5, 9) * tier / 10f);

            // 5-10 plus 10 - 20 per tier
            unit.Mana = gen.Random.Next(5, 11) + gen.Random.Next(10, 21) * tier;


            unit.MineralCost = gen.Random.Next(35, 61) + gen.Random.Next(20, 31) * tier;
            unit.GasCost = Math.Max(0, 25 * (tier - 1));
            unit.SupplyCost = tier;
        }

        private static void PopulateAllRounder(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(1.1);
            unit.Mana = 0;

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.05);

            // attack feature
            {
                int range = gen.Random.Next(1, 4), damageMin = gen.Random.Next(tier * 5, tier * 8), damageMax = damageMin + gen.Random.Next(tier + 1);
                unit.Features.Add(new Attack(range, damageMin, damageMax));
            }

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(4, () =>
            {
                int range = gen.Random.Next(2, 6), radius = gen.Random.Next(4) == 0 ? 3 : 2;
                int damageMin = gen.Random.Next(20 + tier * 7, 20 + tier * 11), damageMax = damageMin + gen.Random.Next(3 * tier + 1);
                return new AreaInstant(range, radius, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(1, () => new Burrow(0)));
            features.Add(new FeatureSelector(6, () =>
            {
                int duration = gen.Random.Next(2, 6), points = gen.Random.Next(2, 5);
                int drain = (int)((unit.Health * 0.65) * (duration + points) / 20);
                return new Stim(duration, drain, points);
            }));

            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // passive features
            features = new List<FeatureSelector>();
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(2, 4));

            // flags
            unit.Flags |= UnitFlags.AttacksGround | UnitFlags.AttacksAir;
        }

        private static void PopulateDamageDealer(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.9);
            unit.Armor = unit.Armor.Scale(0.75);
            unit.Mana = unit.Mana.Scale(0.75);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.05);
            unit.GasCost = unit.GasCost.Scale(1.25);
            unit.SupplyCost = unit.SupplyCost.Scale(1.1);

            // attack feature
            {
                int range = gen.Random.Next(1, 5), damageMin = gen.Random.Next(tier * 8, tier * 11), damageMax = damageMin + gen.Random.Next(tier * 2 + 1);
                unit.Features.Add(new Attack(range, damageMin, damageMax));
            }

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(4, () =>
            {
                int range = gen.Random.Next(2, 6), radius = gen.Random.Next(4) == 0 ? 3 : 2;
                int damageMin = gen.Random.Next(20 + tier * 7, 20 + tier * 11), damageMax = damageMin + gen.Random.Next(3 * tier + 1);
                return new AreaInstant(range, radius, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(2, 6);
                int damageMin = gen.Random.Next(20 + tier * 8, 20 + tier * 12), damageMax = damageMin + gen.Random.Next(3 * tier + 1);
                return new TargettedInstant(range, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(1, () =>
            {
                int damageMin = gen.Random.Next(20 + tier * 7, 20 + tier * 11), damageMax = damageMin + gen.Random.Next(3 * tier + 1);
                return new Landmine(damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(1, () => new PersonalTeleport(gen.Random.Next(3, 4 + tier))));
            features.Add(new FeatureSelector(5, () => new Stim(gen.Random.Next(2, Math.Max(3, tier + 1)), gen.Random.Next(unit.Health / 5, unit.Health / 2), gen.Random.Next(3, 6))));
            features.Add(new FeatureSelector(1, () =>
            {
                int time = Math.Max(1, gen.Random.Next(4));
                int damageMin = gen.Random.Next(20 + tier * 10, 20 + tier * 15), damageMax = damageMin + gen.Random.Next(4 * tier + 1);
                return new Suicide(time, damageMin, damageMax, 1);
            }));
            features.Add(new FeatureSelector(4, () =>
            {
                int pick = gen.Random.Next(4), perTurn = 0, activate = 0;
                if (pick == 0)
                    perTurn = gen.Random.Next(5, 25);
                else if (pick == 1)
                    activate = gen.Random.Next(25, 65).RoundNearest(5);

                return new DeployableAttack(perTurn, activate);
            }));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // passive features
            features = new List<FeatureSelector>();
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(2, 4));

            // flags
            unit.Flags |= UnitFlags.AttacksGround;
            if (unit.Flags.HasFlag(UnitFlags.Flying))
                unit.Flags |= UnitFlags.AttacksAir;
            else if (gen.Random.Next(3) == 0)
                unit.Flags |= UnitFlags.AttacksAir;

            unit.Flags |= UnitFlags.AttacksGround | UnitFlags.AttacksAir;
        }

        private static void PopulateMeatShield(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(1.7);
            unit.Armor = unit.Armor.Scale(2.25);
            unit.Mana = unit.Mana.Scale(0.55);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.2);
            unit.GasCost = unit.GasCost.Scale(1.1);
            unit.SupplyCost = unit.SupplyCost.Scale(1.25);

            // attack feature
            int range = gen.Random.Next(1, 3), damageMin = gen.Random.Next(tier * 3, tier * 6), damageMax = damageMin + gen.Random.Next(tier + 1);
            unit.Features.Add(new Attack(range, damageMin, damageMax));

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(0, 2));

            // passive features
            features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(3, () => new HigherHealth(gen.Random.Next(unit.Health * 15 / 100, unit.Health * 30 / 100).RoundNearest(5))));
            features.Add(new FeatureSelector(3, () => new Armored(gen.Random.Next(tier + 1, tier + 2))));
            features.Add(new FeatureSelector(2, () => new GreaterMobility(gen.Random.Next(tier + 1, tier + 2))));
            features.Add(new FeatureSelector(1, () => new HigherMana(gen.Random.Next(unit.Mana * 15 / 100, unit.Mana * 35 / 100).RoundNearest(5))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // flags
            if (unit.Flags.HasFlag(UnitFlags.Flying))
                unit.Flags |= UnitFlags.AttacksAir;
            else
                unit.Flags |= UnitFlags.AttacksGround;
        }

        private static void PopulateSupportCaster(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.55);
            unit.Armor = 0;

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.5);
            unit.GasCost = unit.GasCost.Scale(2) + 25;
            unit.SupplyCost = unit.SupplyCost.Scale(0.75);

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(2, () => 
            {
                int range = gen.Random.Next(3, 6), duration = gen.Random.Next(2) == 0 ? 0 : gen.Random.Next(3,10);
                int damageMin = gen.Random.Next(tier * 2, tier * 5), damageMax = damageMin + gen.Random.Next(tier + 1);
                return new TargettedDoT(range, duration, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(4, () =>
            {
                int range = gen.Random.Next(1, 6);
                int damageMin = gen.Random.Next(tier * 4, tier * 6), damageMax = damageMin + gen.Random.Next(tier + 1);
                return new InstantHeal(range, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(4, () =>
            {
                int range = gen.Random.Next(1, 4), duration = gen.Random.Next(3, 6);
                int damageMin = gen.Random.Next(tier * 1, tier * 4), damageMax = damageMin + gen.Random.Next(tier + 1);
                return new HealOverTime(range, duration, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(3, () => new Blind(gen.Random.Next(3,7), gen.Random.Next(2) == 0 ? 0 : gen.Random.Next(4, 10))));
            features.Add(new FeatureSelector(4, () => new RemoveEffects(gen.Random.Next(1, 5))));
            features.Add(new FeatureSelector(4, () => 
            {
                int range = gen.Random.Next(1, 4), duration = gen.Random.Next(4, 10), healthBoost = 0, armorBoost = 0;
                if (gen.Random.Next(5) < 2)
                    armorBoost = gen.Random.Next(3, 10);
                else
                    healthBoost = gen.Random.Next(30, 105).RoundNearest(5);
                return new HealthBoost(range, duration, healthBoost, armorBoost);
            }));
            features.Add(new FeatureSelector(2, () => new Freeze(gen.Random.Next(3, 7), 1, gen.Random.Next(3, 6))));
            features.Add(new FeatureSelector(2, () => new Slow(gen.Random.Next(3, 7), gen.Random.Next(2, 4), gen.Random.Next(4, 8), gen.Random.Next(3, 6))));
            features.Add(new FeatureSelector(2, () => new Immobilize(gen.Random.Next(3, 7), 1, gen.Random.Next(4, 7))));
            features.Add(new FeatureSelector(4, () => new Wall(gen.Random.Next(1, 5), gen.Random.Next(4, 10))));
            features.Add(new FeatureSelector(1, () => new Possession(gen.Random.Next(1, 4))));
            features.Add(new FeatureSelector(2, () => new DrainOwnHealth(gen.Random.Next(1, 4), gen.Random.Next(unit.Health / 10, unit.Health / 5), gen.Random.Next(10, 31) / 10f)));
            features.Add(new FeatureSelector(3, () => new AreaDetection(gen.Random.Next(5, 12), gen.Random.Next(2, 6), gen.Random.Next(1, 3))));
            features.Add(new FeatureSelector(3, () => new StealVision(gen.Random.Next(5, 10), gen.Random.Next(3) == 0 ? 0 : gen.Random.Next(3, 12))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(3, 4));

            // passive features
            features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(1, () => new HigherMana(gen.Random.Next(unit.Mana * 15 / 100, unit.Mana * 35 / 100).RoundNearest(5))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 2));
        }

        private static void PopulateOffensiveCaster(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.7);
            unit.Armor = unit.Armor.Scale(0.3);
            unit.Mana = unit.Mana.Scale(1.15);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.55);
            unit.GasCost = unit.GasCost.Scale(2.25) + 25;
            unit.SupplyCost = unit.SupplyCost.Scale(0.9);

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(5, () =>
            {
                int range = gen.Random.Next(1, 4), radius = gen.Random.Next(2,5), duration = gen.Random.Next(3, 6);
                int damageMin = gen.Random.Next(tier * 1, tier * 4), damageMax = damageMin + gen.Random.Next(tier + 1);
                return new AreaDoT(range, radius, duration, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(4, () =>
            {
                int range = gen.Random.Next(3, 6), duration = gen.Random.Next(2) == 0 ? 0 : gen.Random.Next(3, 10);
                int damageMin = gen.Random.Next(tier * 2, tier * 5), damageMax = damageMin + gen.Random.Next(tier + 1);
                return new TargettedDoT(range, duration, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(3, () => new Blind(gen.Random.Next(3, 7), gen.Random.Next(2) == 0 ? 0 : gen.Random.Next(4, 10))));
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(1, 4), duration = gen.Random.Next(4, 10), healthBoost = 0, armorBoost = 0;
                if (gen.Random.Next(5) < 2)
                    armorBoost = gen.Random.Next(3, 10);
                else
                    healthBoost = gen.Random.Next(30, 105).RoundNearest(5);
                return new HealthBoost(range, duration, healthBoost, armorBoost);
            }));
            features.Add(new FeatureSelector(5, () =>
            {
                int range = gen.Random.Next(4, 10), radius = gen.Random.Next(3, 6);
                int damageMin = gen.Random.Next(tier * 15, tier * 30), damageMax = damageMin + gen.Random.Next(tier + 1);
                return new AreaManaDrain(range, radius, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(4, () => new Freeze(gen.Random.Next(3, 7), 1, gen.Random.Next(3, 6))));
            features.Add(new FeatureSelector(4, () => new Slow(gen.Random.Next(3, 7), gen.Random.Next(2, 4), gen.Random.Next(4, 8), gen.Random.Next(3, 6))));
            features.Add(new FeatureSelector(4, () => new Immobilize(gen.Random.Next(3, 7), 1, gen.Random.Next(4, 7))));
            features.Add(new FeatureSelector(2, () => new Cloaking_AOE_ManaDrain(gen.Random.Next(10, 26), gen.Random.Next(30, 60), gen.Random.Next(2,5))));
            features.Add(new FeatureSelector(2, () => new Cloaking_AOE_Permanent(gen.Random.Next(2, 5))));
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(1, 4), radius = gen.Random.Next(2,5), duration = gen.Random.Next(4, 10), healthBoost = 0, armorBoost = 0;
                if (gen.Random.Next(5) < 2)
                    armorBoost = gen.Random.Next(6, 10);
                else
                    healthBoost = gen.Random.Next(75, 180).RoundNearest(5);
                return new AreaShield(range, radius, duration, healthBoost, armorBoost);
            }));
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(1, 5), duration = 0, controlRange = 0;
                int mode = gen.Random.Next(3);
                if (mode == 0)
                    duration = gen.Random.Next(5, 14);
                else if (mode == 1)
                    controlRange = gen.Random.Next(4, 12);

                return new MindControl(range, duration, controlRange);
            }));
            features.Add(new FeatureSelector(3, () => new ManaBurn(gen.Random.Next(3, 6), gen.Random.Next(50, 200).RoundNearest(10), gen.Random.Next(12, 30) / 10f)));
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(3) == 0 ? 0 : gen.Random.Next(12, 30), radius = gen.Random.Next(2, 5);
                bool friendlyOnly = gen.Random.Next(3) != 0;
                return new MassTeleport(range, radius, friendlyOnly);
            }));
            features.Add(new FeatureSelector(2, () => new KillForMana(gen.Random.Next(1, 6), gen.Random.Next(12, 30) / 10f)));
            features.Add(new FeatureSelector(4, () =>
            {
                int range = gen.Random.Next(3, 6), duration = gen.Random.Next(2, 6);
                int damagePerTurn = gen.Random.Next(15, 32).RoundNearest(5);
                float manaPerHitpoint = gen.Random.Next(5, 12) / 10f;
                return new DrainHealth(range, duration, damagePerTurn, manaPerHitpoint);
            }));
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(3, 7), radius = gen.Random.Next(2, 5), duration = gen.Random.Next(3, 7);
                int hitpointsDrained = gen.Random.Next(70, 200).RoundNearest(5), minHitpoints = 1, drainPerTurn = gen.Random.Next(20, 48).RoundNearest(5);
                return new AreaHealthReduction(range, radius, duration, hitpointsDrained, minHitpoints, drainPerTurn);
            }));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(3, 4));

            // passive features
            features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(1, () => new HigherMana(gen.Random.Next(unit.Mana * 15 / 100, unit.Mana * 35 / 100).RoundNearest(5))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));
        }

        private static void PopulateScout(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.7);
            unit.Armor = unit.Armor.Scale(0.45);
            unit.Mana = unit.Mana.Scale(0.65);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.6);
            unit.GasCost = unit.GasCost.Scale(0.7);
            unit.SupplyCost = unit.SupplyCost.Scale(0.7);

            // attack feature
            if (gen.Random.Next(4) != 0)
            {
                int range = gen.Random.Next(1, 4), damageMin = gen.Random.Next(tier * 3, tier * 6), damageMax = damageMin + gen.Random.Next(tier + 1);
                unit.Features.Add(new Attack(range, damageMin, damageMax));
            }
            else
                unit.Features.Add(new Cloaking_Permanent());

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(3, () =>
            {
                int damageMin = gen.Random.Next(20 + tier * 7, 20 + tier * 11), damageMax = damageMin + gen.Random.Next(3 * tier + 1);
                return new Landmine(damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(2, () => new Immobilize(gen.Random.Next(3, 7), 1, gen.Random.Next(4, 7))));
            features.Add(new FeatureSelector(4, () => new Wall(gen.Random.Next(1, 5), gen.Random.Next(4, 10))));
            features.Add(new FeatureSelector(2, () => new Cloaking_ManaDrain(gen.Random.Next(10, 26), gen.Random.Next(30, 60))));
            features.Add(new FeatureSelector(4, () => new Burrow(gen.Random.Next(2) == 0 ? 0 : gen.Random.Next(20, 50).RoundNearest(5))));
            features.Add(new FeatureSelector(2, () => new PersonalTeleport(gen.Random.Next(3, 4 + tier))));
            features.Add(new FeatureSelector(3, () => new AreaDetection(gen.Random.Next(5, 12), gen.Random.Next(2, 6), gen.Random.Next(1, 3))));
            features.Add(new FeatureSelector(3, () => new StealVision(gen.Random.Next(5, 10), gen.Random.Next(3) == 0 ? 0 : gen.Random.Next(3, 12))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // passive features
            features = new List<FeatureSelector>();
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(2, 4));

            // flags
            unit.Flags |= UnitFlags.AttacksGround;
            if (gen.Random.Next(3) == 0)
                unit.Flags |= UnitFlags.AttacksAir;
        }

        private static void PopulateInfiltrator(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.85);
            unit.Armor = unit.Armor.Scale(0.7);
            unit.Mana = unit.Mana.Scale(0.8);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(1.2);
            unit.GasCost = unit.GasCost.Scale(1.6);
            unit.SupplyCost = unit.SupplyCost.Scale(1.4);

            // attack feature
            {
                int range = gen.Random.Next(1, 3), damageMin = gen.Random.Next(tier * 3, tier * 6), damageMax = damageMin + gen.Random.Next(tier + 1);
                unit.Features.Add(new Attack(range, damageMin, damageMax));
            }

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(3, () =>
            {
                int range = gen.Random.Next(2, 6);
                int damageMin = gen.Random.Next(20 + tier * 8, 20 + tier * 12), damageMax = damageMin + gen.Random.Next(3 * tier + 1);
                return new TargettedInstant(range, damageMin, damageMax);
            }));
            features.Add(new FeatureSelector(2, () => new Freeze(gen.Random.Next(3, 7), 1, gen.Random.Next(3, 6))));
            features.Add(new FeatureSelector(6, () => new Cloaking_ManaDrain(gen.Random.Next(10, 26), gen.Random.Next(30, 60))));
            features.Add(new FeatureSelector(6, () => new Cloaking_Permanent()));
            features.Add(new FeatureSelector(4, () => new ManaBurn(gen.Random.Next(3, 6), gen.Random.Next(50, 200).RoundNearest(10), gen.Random.Next(12, 30) / 10f)));
            features.Add(new FeatureSelector(4, () => new PersonalTeleport(gen.Random.Next(3, 4 + tier))));
            features.Add(new FeatureSelector(3, () => new StealVision(gen.Random.Next(5, 10), gen.Random.Next(3) == 0 ? 0 : gen.Random.Next(3, 12))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(2, 4));

            // passive features
            features = new List<FeatureSelector>();
            features.Add(new FeatureSelector(2, () => new GreaterMobility(gen.Random.Next(tier + 1, tier + 2))));
            features.Add(new FeatureSelector(3, () => new GreaterVisibility(gen.Random.Next(1, 4))));
            features.Add(new FeatureSelector(5, () => new HigherMana(gen.Random.Next(unit.Health * 15 / 100, unit.Health * 35 / 100).RoundNearest(5))));
            features.Add(new FeatureSelector(1, () => new Detector()));
            features.Add(new FeatureSelector(1, () => new Supply(gen.Random.Next(2, 5))));
            features.Add(new FeatureSelector(2, () => new LongerRange(gen.Random.Next(1, 4))));
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // flags
            unit.Flags |= UnitFlags.AttacksGround;
            if (gen.Random.Next(5) == 0)
                unit.Flags |= UnitFlags.AttacksAir;

            if (gen.Random.Next(4) == 0)
                unit.Flags |= UnitFlags.Agile;
        }

        private static void PopulateTransport(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(1.05);
            unit.Armor = unit.Armor.Scale(1.25);
            unit.Mana = unit.Mana.Scale(0.5);

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.65);
            unit.GasCost = unit.GasCost.Scale(0.25);
            unit.SupplyCost = unit.SupplyCost.Scale(0.45);

            unit.Features.Add(new Carrier(gen.Random.Next(3, 9)));

            // active features
            List<FeatureSelector> features = new List<FeatureSelector>();
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // passive features
            features = new List<FeatureSelector>();
            AllocateFeatures(gen, unit, features, tier, gen.Random.Next(2, 4));

            // flags
            unit.Flags |= UnitFlags.Flying;
        }

        private static void PopulateWorker(OldTreeGenerator gen, UnitType unit, int tier)
        {
            // scale stats
            unit.Health = unit.Health.Scale(0.75);
            unit.Armor = 0;
            unit.Mana = 0;

            // scale costs
            unit.MineralCost = unit.MineralCost.Scale(0.55);
            unit.GasCost = 0;
            unit.SupplyCost = 1;

            // attack feature
            int range = gen.Random.Next(1, 2), damageMin = gen.Random.Next(3, 6), damageMax = damageMin + gen.Random.Next(3);
            unit.Features.Add(new Attack(range, damageMin, damageMax));

            unit.Features.Add(new Construction());

            // active features
            // List<Selector> features = new List<Selector>();
            // AllocateFeatures(gen, unit, features, tier, gen.Random.Next(1, 3));

            // passive features
            // features = new List<Selector>();
            // AllocateFeatures(gen, unit, features, tier, gen.Random.Next(2, 4));
    }

    private class FeatureSelector
        {
            public FeatureSelector(int weight, Func<Feature> feature)
            {
                Weight = weight;
                Select = feature;
            }

            public Func<Feature> Select { get; private set; }
            public int Weight { get; private set; }
        }

        private static void AllocateFeatures(OldTreeGenerator gen, UnitType unit, List<FeatureSelector> features, int tier, int numFeatures)
        {
            int total = features.Select(f => f.Weight).Sum();

            for (int i = 0; i < numFeatures; i++)
            {
                int selected = gen.Random.Next(total);

                int cumulative = 0;
                foreach (var feature in features)
                {
                    cumulative += feature.Weight;

                    if (cumulative > selected)
                    {
                        AddFeature(gen, unit, feature.Select(), unit.Prerequisite, tier);
                        total -= feature.Weight;
                        features.Remove(feature);
                        break;
                    }
                }
            }
        }

        private static void AddFeature(OldTreeGenerator gen, UnitType unit, Feature feature, BuildingType prerequisite, int tier)
        {
            unit.Features.Add(feature);
            double costScale = feature.Initialize(unit);

            unit.MineralCost = (int)(unit.MineralCost * costScale);
            unit.GasCost = (int)(unit.GasCost * costScale);

            if (prerequisite != null && unit.Features.Count > 1)
            {// a unit's first feature never requires an unlock
                
                // research should be done at pure-research buildings, not at factories. If unit's prerequisite is a factory, give its research to a research building under that factory instead.
                BuildingType researchBuilding;
                if (prerequisite.Builds.Count == 0)
                    researchBuilding = prerequisite;
                else
                {
                    researchBuilding = prerequisite.Unlocks.FirstOrDefault(u => u is BuildingType && (u as BuildingType).Builds.Count == 0) as BuildingType;
                    if (researchBuilding == null)
                        researchBuilding = prerequisite;
                }

                feature.UnlockedBy = Research.CreateForFeature(gen, researchBuilding, feature, tier);
            }
        }
        */
    }
}
