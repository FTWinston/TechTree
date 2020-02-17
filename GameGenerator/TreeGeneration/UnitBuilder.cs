using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameGenerator.TreeGeneration
{
    public class UnitBuilder : EntityBuilder, IUnitType
    {
        public UnitBuilder(Random random, uint id, string symbol)
            : base(random, id, symbol)
        {
            Name = "Unnamed unit";
        }

        public UnitFlags Flags { get; set; }

        public UnitMobility Mobility { get; set; } = UnitMobility.Standard;

        public uint BuiltBy { get; set; }

        public int MoveRange { get; set; } = 3;

        public override void AllocateName(ICollection<string> usedNames)
        {
            Name = DetermineUniqueName(usedNames, "Unit");
        }

        public UnitType Build()
        {
            /*
            // 10-15, plus 13-20 per tier
            int health = Random.Next(10, 16) + Random.Next(13, 21) * Tier
                .RoundNearest(5);

            // ~1/3 chance of 1, plus 0.5 to 0.8 per tier
            int armor = ((Random.NextDouble(0.3, 0.6) + Random.NextDouble(0.5, 0.9) * Tier))
                .RoundNearest(1);

            unit.VisionRange = UnitVisionRange;
            MoveType = Standard, Cumbersome or Versatile
            unit.BuildTime = Random.Next(Math.Max(1, tier - 2), Tier + 2);
            unit.Mana = Random.Next(5, 11) + Random.Next(10, 21) * Tier;


            unit.MineralCost = Random.Next(35, 61) + Random.Next(20, 31) * Tier;
            unit.VespineCost = Math.Max(0, 25 * (Tier - 1));
            unit.SupplyCost = Tier;
            */

            return new UnitType(this);
        }
    }
}
