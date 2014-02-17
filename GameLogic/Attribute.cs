using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public abstract class Attribute
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public float SelectionChance { get; set; }

        // training units, passive upgrades, and spells can all have Building or Research prerequisites.
        // actually, typically, only training units & building buildings can have building prerequisites, whereas everything can have research prerequisites.
        public List<BuyableInfo> Prerequisites { get; set; }
        
        protected Attribute()
        {
            Prerequisites = new List<BuyableInfo>();
            Value = 1; SelectionChance = 1;
        }
    }

    public class Passive : Attribute
    {
        // modifies stats, possibly adds new capabilities. Can be hidden (e.g. "more hitpoints") or not ("regeneration").
        public bool Hidden { get; set; }
    }

    public abstract class Ability : Attribute
    {
        public int MineralCost { get; set; }
        public int GasCost { get; set; }
        public int ManaCost { get; set; }
    }

    public class TrainUnit : Ability
    {
        public float BuildTime { get; set; }
        public UnitInfo UnitTrained { get; set; }
    }

    public class ConstructBuilding : Ability
    {
        public float BuildTime { get; set; }
        public BuildingInfo BuildingProduced { get; set; }
    }

    public class Spell : Ability
    {
        public float ChargeTime { get; set; }
        public float CooldownTime { get; set; }
        public int MaxCastNum { get; set; }

        // todo: implement this
    }

    public class Research : Ability
    {
        public int NumRanks { get; set; }
        
        public float BuildTime { get; set; }
        public float BuildTimePerRank { get; set; }

        public int MineralCostPerRank { get; set; }
        public int GasCostPerRank { get; set; }
        public int ManaCostPerRank { get; set; }

        //TODO: fix this. err. does this really need a separate class? Only for the purpose of being a prerequisite.
        public ResearchInfo ResearchProduced { get; set; }
    }
}

/*
When generating a unit, we're given a BUDGET, and it adds attributes costing less than the remaining value budget, until it can't add more.


How do we differentiate between reavers BUILDING scarabs (and only carrying that entity type), and infestors summoning infested terrans?
Perhaps there isn't a difference, apart from the delay and the cost type. On the other hand, one can be queued, the other can't. Yeah one is a BUILD, the other a SUMMON.
Can we think of a queuable BUILD scenario that isn't a "build and hold"? Uh, yeah, I guess.
*/