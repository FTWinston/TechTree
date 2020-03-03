using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.Features;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class FeatureDTO
    {
        public FeatureDTO(string name, string symbol, string type, Dictionary<string, int> data)
        {
            Name = name;
            Symbol = symbol;
            Type = type;
            Data = data;
        }

        public string Name { get; }

        public string Symbol { get; }

        public string Type { get; }

        public Dictionary<string, int> Data { get; }

        public Feature ToFeature()
        {
            return Type switch
            {
                AreaDetection.TypeID => new AreaDetection(Name, Symbol, Data),
                AreaDoT.TypeID => new AreaDoT(Name, Symbol, Data),
                AreaHealthReduction.TypeID => new AreaHealthReduction(Name, Symbol, Data),
                AreaManaDrain.TypeID => new AreaManaDrain(Name, Symbol, Data),
                AreaShield.TypeID => new AreaShield(Name, Symbol, Data),
                Attack.TypeID => new Attack(Name, Symbol, Data),
                Blind.TypeID => new Blind(Name, Symbol, Data),
                Build.TypeID => new Build(Name, Symbol, Data),
                Burrow.TypeID => new Burrow(Name, Symbol, Data),
                Carrier.TypeID => new Carrier(Name, Symbol, Data),
                Cloaking_AOE_ManaDrain.TypeID => new Cloaking_AOE_ManaDrain(Name, Symbol, Data),
                Cloaking_AOE_Permanent.TypeID => new Cloaking_AOE_Permanent(Name, Symbol, Data),
                Cloaking_ManaDrain.TypeID => new Cloaking_ManaDrain(Name, Symbol, Data),
                Cloaking_Permanent.TypeID => new Cloaking_Permanent(Name, Symbol, Data),
                Construction.TypeID => new Construction(Name, Symbol, Data),
                DeployableAttack.TypeID => new DeployableAttack(Name, Symbol, Data),
                DrainHealth.TypeID => new DrainHealth(Name, Symbol, Data),
                DrainOwnHealth.TypeID => new DrainOwnHealth(Name, Symbol, Data),
                Freeze.TypeID => new Freeze(Name, Symbol, Data),
                HealOverTime.TypeID => new HealOverTime(Name, Symbol, Data),
                HealthBoost.TypeID => new HealthBoost(Name, Symbol, Data),
                Immobilize.TypeID => new Immobilize(Name, Symbol, Data),
                InstantHeal.TypeID => new InstantHeal(Name, Symbol, Data),
                KillForMana.TypeID => new KillForMana(Name, Symbol, Data),
                Landmine.TypeID => new Landmine(Name, Symbol, Data),
                ManaBurn.TypeID => new ManaBurn(Name, Symbol, Data),
                MassTeleport.TypeID => new MassTeleport(Name, Symbol, Data),
                MindControl.TypeID => new MindControl(Name, Symbol, Data),
                HigherHealth.TypeID => new HigherHealth(Name, Symbol, Data),
                Armored.TypeID => new Armored(Name, Symbol, Data),
                GreaterMobility.TypeID => new GreaterMobility(Name, Symbol, Data),
                GreaterVisibility.TypeID => new GreaterVisibility(Name, Symbol, Data),
                HigherMana.TypeID => new HigherMana(Name, Symbol, Data),
                Detector.TypeID => new Detector(Name, Symbol, Data),
                Supply.TypeID => new Supply(Name, Symbol, Data),
                LongerRange.TypeID => new LongerRange(Name, Symbol, Data),
                PerformResearch.TypeID => new PerformResearch(Name, Symbol, Data),
                PersonalTeleport.TypeID => new PersonalTeleport(Name, Symbol, Data),
                Possession.TypeID => new Possession(Name, Symbol, Data),
                RemoveEffects.TypeID => new RemoveEffects(Name, Symbol, Data),
                Slow.TypeID => new Slow(Name, Symbol, Data),
                StealVision.TypeID => new StealVision(Name, Symbol, Data),
                Stim.TypeID => new Stim(Name, Symbol, Data),
                Suicide.TypeID => new Suicide(Name, Symbol, Data),
                TargettedDoT.TypeID => new TargettedDoT(Name, Symbol, Data),
                TargettedInstant.TypeID => new TargettedInstant(Name, Symbol, Data),
                Wall.TypeID => new Wall(Name, Symbol, Data),
                _ => throw new ArgumentOutOfRangeException($"Unable to load feature with unrecognised type: {Type}"),
            };
        }
    }
}
