using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels
{
    public enum ObjectiveFeature
    {
        Units,
        ResourceReserves,
        RevealObjectives,
    }

    public class Objective
    {
        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }

        public ObjectiveFeature Feature { get; set; }

        public uint? FeatureTypeID { get; set; }

        public int TargetQuantity { get; set; }

        public bool RelativeToOpponent { get; set; }

        public Dictionary<int, int[]>? CellsByPlayer { get; set; }

        public bool IsSatisfied(Game game, Player player)
        {
            switch (Feature)
            {
                case ObjectiveFeature.Units:
                    return IsUnitQuantitySatisfied(game, player);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool IsUnitQuantitySatisfied(Game game, Player player)
        {
            Func<Unit, bool> unitFilter = unit => true;

            if (FeatureTypeID.HasValue)
            {
                unitFilter = unit => /*unitFilter(unit) &&*/ unit.Definition.ID == FeatureTypeID.Value;
            }

            if (CellsByPlayer != null)
            {
                unitFilter = unit => unitFilter(unit) && CellsByPlayer[unit.Owner.ID].Contains(unit.Location.ID);
            }

            int quantity = player.Units.Count(unitFilter);

            if (RelativeToOpponent)
            {
                int opponentQuantity = game.Players
                    .Where(opponent => opponent != player)
                    .Max(opponent => opponent.Units.Count(unitFilter));

                quantity -= opponentQuantity;
            }

            return quantity >= TargetQuantity;
        }
    }
}
