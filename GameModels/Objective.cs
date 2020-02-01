using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModels
{
    public class Objective
    {
        public string Description { get; set; }

        public int Value { get; set; }

        public uint? UnitTypeID { get; set; }

        public int TargetQuantity { get; set; }

        public bool RelativeToOpponent { get; set; }

        public Dictionary<int, int[]> CellsByPlayer { get; set; }

        public bool IsSatisfied(Game game, Player player)
        {
            Func<Unit, bool> unitFilter = unit => true;

            if (UnitTypeID.HasValue)
            {
                unitFilter = unit => /*unitFilter(unit) &&*/ unit.Definition.ID == UnitTypeID.Value;
            }

            if (CellsByPlayer != null)
            {
                // unitFilter = unit => unitFilter(unit) && CellsByPlayer[unit.Owner.ID].Contains(unit.Location.ID);
                // TODO: fix this ugly hack. Let cells have an ID?
                unitFilter = unit => unitFilter(unit) && CellsByPlayer[unit.Owner.ID].Any(cellID => game.Battlefield.Cells[cellID]?.Entity == unit);
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
