using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.Features;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Serialization
{
    internal class FeatureDTO
    {
        public FeatureDTO(uint id, string name, string symbol, string type, Dictionary<string, int> data)
        {
            ID = id;
            Name = name;
            Symbol = symbol;
            Type = type;
            Data = data;
        }

        public uint ID { get; }

        public string Name { get; }

        public string Symbol { get; }

        public string Type { get; }

        public Dictionary<string, int> Data { get; }

        public Feature ToFeature()
        {
            return Type switch
            {
                AreaDetection.TypeID => new AreaDetection(ID, Name, Symbol, Data),
                AreaDoT.TypeID => new AreaDoT(ID, Name, Symbol, Data),
                AreaHealthReduction.TypeID => new AreaHealthReduction(ID, Name, Symbol, Data),
                AreaManaDrain.TypeID => new AreaManaDrain(ID, Name, Symbol, Data),
                AreaShield.TypeID => new AreaShield(ID, Name, Symbol, Data),
                Attack.TypeID => new Attack(ID, Name, Symbol, Data),
                Blind.TypeID => new Blind(ID, Name, Symbol, Data),
                Build.TypeID => new Build(ID, Name, Symbol, Data),
                Burrow.TypeID => new Burrow(ID, Name, Symbol, Data),
                Carrier.TypeID => new Carrier(ID, Name, Symbol, Data),
                Cloaking_AOE_ManaDrain.TypeID => new Cloaking_AOE_ManaDrain(ID, Name, Symbol, Data),
                Cloaking_AOE_Permanent.TypeID => new Cloaking_AOE_Permanent(ID, Name, Symbol, Data),
                Cloaking_ManaDrain.TypeID => new Cloaking_ManaDrain(ID, Name, Symbol, Data),
                Cloaking_Permanent.TypeID => new Cloaking_Permanent(ID, Name, Symbol, Data),
                Construction.TypeID => new Construction(ID, Name, Symbol, Data),
                DeployableAttack.TypeID => new DeployableAttack(ID, Name, Symbol, Data),
                DrainHealth.TypeID => new DrainHealth(ID, Name, Symbol, Data),
                DrainOwnHealth.TypeID => new DrainOwnHealth(ID, Name, Symbol, Data),
                Freeze.TypeID => new Freeze(ID, Name, Symbol, Data),
                HealOverTime.TypeID => new HealOverTime(ID, Name, Symbol, Data),
                HealthBoost.TypeID => new HealthBoost(ID, Name, Symbol, Data),
                Immobilize.TypeID => new Immobilize(ID, Name, Symbol, Data),
                InstantHeal.TypeID => new InstantHeal(ID, Name, Symbol, Data),
                KillForMana.TypeID => new KillForMana(ID, Name, Symbol, Data),
                Landmine.TypeID => new Landmine(ID, Name, Symbol, Data),
                ManaBurn.TypeID => new ManaBurn(ID, Name, Symbol, Data),
                MassTeleport.TypeID => new MassTeleport(ID, Name, Symbol, Data),
                MindControl.TypeID => new MindControl(ID, Name, Symbol, Data),
                HigherHealth.TypeID => new HigherHealth(ID, Name, Symbol, Data),
                Armored.TypeID => new Armored(ID, Name, Symbol, Data),
                GreaterMobility.TypeID => new GreaterMobility(ID, Name, Symbol, Data),
                GreaterVisibility.TypeID => new GreaterVisibility(ID, Name, Symbol, Data),
                HigherMana.TypeID => new HigherMana(ID, Name, Symbol, Data),
                Detector.TypeID => new Detector(ID, Name, Symbol, Data),
                Supply.TypeID => new Supply(ID, Name, Symbol, Data),
                LongerRange.TypeID => new LongerRange(ID, Name, Symbol, Data),
                PerformResearch.TypeID => new PerformResearch(ID, Name, Symbol, Data),
                PersonalTeleport.TypeID => new PersonalTeleport(ID, Name, Symbol, Data),
                Possession.TypeID => new Possession(ID, Name, Symbol, Data),
                RemoveEffects.TypeID => new RemoveEffects(ID, Name, Symbol, Data),
                Slow.TypeID => new Slow(ID, Name, Symbol, Data),
                StealVision.TypeID => new StealVision(ID, Name, Symbol, Data),
                Stim.TypeID => new Stim(ID, Name, Symbol, Data),
                Suicide.TypeID => new Suicide(ID, Name, Symbol, Data),
                TargettedDoT.TypeID => new TargettedDoT(ID, Name, Symbol, Data),
                TargettedInstant.TypeID => new TargettedInstant(ID, Name, Symbol, Data),
                Wall.TypeID => new Wall(ID, Name, Symbol, Data),
                _ => throw new ArgumentOutOfRangeException($"Unable to load feature with unrecognised type: {Type}"),
            };
        }
    }
}
