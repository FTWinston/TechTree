using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.Features;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class FeatureDTO
    {
        public FeatureDTO(string type, Dictionary<string, int> data)
        {
            Type = type;
            Data = data;
        }

        public string Type { get; }

        public Dictionary<string, int> Data { get; }

        public Feature ToFeature()
        {
            return Type switch
            {
                AreaDetection.TypeID => new AreaDetection(Data),
                AreaDoT.TypeID => new AreaDoT(Data),
                AreaHealthReduction.TypeID => new AreaHealthReduction(Data),
                AreaManaDrain.TypeID => new AreaManaDrain(Data),
                AreaShield.TypeID => new AreaShield(Data),
                Attack.TypeID => new Attack(Data),
                Blind.TypeID => new Blind(Data),
                Build.TypeID => new Build(Data),
                Burrow.TypeID => new Burrow(Data),
                Carrier.TypeID => new Carrier(Data),
                Cloaking_AOE_ManaDrain.TypeID => new Cloaking_AOE_ManaDrain(Data),
                Cloaking_AOE_Permanent.TypeID => new Cloaking_AOE_Permanent(Data),
                Cloaking_ManaDrain.TypeID => new Cloaking_ManaDrain(Data),
                Cloaking_Permanent.TypeID => new Cloaking_Permanent(Data),
                Construction.TypeID => new Construction(Data),
                DeployableAttack.TypeID => new DeployableAttack(Data),
                DrainHealth.TypeID => new DrainHealth(Data),
                DrainOwnHealth.TypeID => new DrainOwnHealth(Data),
                Freeze.TypeID => new Freeze(Data),
                HealOverTime.TypeID => new HealOverTime(Data),
                HealthBoost.TypeID => new HealthBoost(Data),
                Immobilize.TypeID => new Immobilize(Data),
                InstantHeal.TypeID => new InstantHeal(Data),
                KillForMana.TypeID => new KillForMana(Data),
                Landmine.TypeID => new Landmine(Data),
                ManaBurn.TypeID => new ManaBurn(Data),
                MassTeleport.TypeID => new MassTeleport(Data),
                MindControl.TypeID => new MindControl(Data),
                HigherHealth.TypeID => new HigherHealth(Data),
                Armored.TypeID => new Armored(Data),
                GreaterMobility.TypeID => new GreaterMobility(Data),
                GreaterVisibility.TypeID => new GreaterVisibility(Data),
                HigherMana.TypeID => new HigherMana(Data),
                Detector.TypeID => new Detector(Data),
                Supply.TypeID => new Supply(Data),
                LongerRange.TypeID => new LongerRange(Data),
                PerformResearch.TypeID => new PerformResearch(Data),
                PersonalTeleport.TypeID => new PersonalTeleport(Data),
                Possession.TypeID => new Possession(Data),
                RemoveEffects.TypeID => new RemoveEffects(Data),
                Slow.TypeID => new Slow(Data),
                StealVision.TypeID => new StealVision(Data),
                Stim.TypeID => new Stim(Data),
                Suicide.TypeID => new Suicide(Data),
                TargettedDoT.TypeID => new TargettedDoT(Data),
                TargettedInstant.TypeID => new TargettedInstant(Data),
                Wall.TypeID => new Wall(Data),
                _ => throw new ArgumentOutOfRangeException($"Unable to load feature with unrecognised type: {Type}"),
            };
        }
    }
}
