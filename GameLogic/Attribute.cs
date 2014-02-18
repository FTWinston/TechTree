using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public abstract class Attribute
    {
        public string Name { get; internal set; }
        public float Value { get; internal set; }
        public float SelectionChance { get; internal set; }

        // training units, passive upgrades, and spells can all have Building or Research prerequisites.
        // actually, typically, only training units & building buildings can have building prerequisites, whereas everything can have research prerequisites.
        public List<BuyableInfo> Prerequisites { get; internal set; }
        
        protected Attribute()
        {
            Prerequisites = new List<BuyableInfo>();
            Value = 1; SelectionChance = 1;
        }
    }

    public class Passive : Attribute
    {
        // modifies stats, possibly adds new capabilities. Can be hidden (e.g. "more hitpoints") or not ("regeneration").
        public bool Hidden { get; internal set; }
    }

    public abstract class Ability : Attribute
    {
        public int MineralCost { get; internal set; }
        public int GasCost { get; internal set; }
        public int ManaCost { get; internal set; }
    }

    public class TrainUnit : Ability
    {
        public float BuildTime { get; internal set; }
        public UnitInfo UnitTrained { get; internal set; }
    }

    public class ConstructBuilding : Ability
    {
        public float BuildTime { get; internal set; }
        public BuildingInfo BuildingProduced { get; internal set; }
    }

    public class Spell : Ability
    {
        public float ChargeTime { get; internal set; }
        public float CooldownTime { get; internal set; }
        public int MaxCastNum { get; internal set; }

        // todo: implement this
    }

    public class Research : Ability
    {
        public int NumRanks { get; internal set; }

        public float BuildTime { get; internal set; }
        public float BuildTimePerRank { get; internal set; }

        public int MineralCostPerRank { get; internal set; }
        public int GasCostPerRank { get; internal set; }
        public int ManaCostPerRank { get; internal set; }

        //TODO: fix this. err. does this really need a separate class? Only for the purpose of being a prerequisite.
        public ResearchInfo ResearchProduced { get; internal set; }
    }
}

/*
When generating a unit, we're given a BUDGET, and it adds attributes costing less than the remaining value budget, until it can't add more.


How do we differentiate between reavers BUILDING scarabs (and only carrying that entity type), and infestors summoning infested terrans?
Perhaps there isn't a difference, apart from the delay and the cost type. On the other hand, one can be queued, the other can't. Yeah one is a BUILD, the other a SUMMON.
Can we think of a queuable BUILD scenario that isn't a "build and hold"? Uh, yeah, I guess.
*/