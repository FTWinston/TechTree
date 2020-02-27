using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class VisionService
    {
        private static HashSet<TCell> GetVisibleCells<TCell>(IGraph<TCell> map, TCell from, int range, Func<TCell, bool> blocksVision)
        {
            var visible = new HashSet<TCell> { from };

            foreach (var targetCell in map.GetCellsWithinDistance(from, range))
            {
                foreach (var testCell in map.TraceLine(from, targetCell))
                {
                    visible.Add(testCell);

                    if (blocksVision(testCell))
                        break;
                }
            }

            return visible;
        }

        private HashSet<Cell> GetVisibleCells(IGraph<Cell> map, Cell from, int range)
        {
            return GetVisibleCells(map, from, range, cell => cell.BlocksVision);
        }

        public void UpdateVisionForMoveStep
        (
            Battlefield battlefield,
            Unit unit,
            Cell fromCell,
            Cell toCell,
            out HashSet<Cell> cellsRevealed,
            out HashSet<Cell> cellsHidden
        )
        {
            int visionRange = unit.Definition.VisionRange;

            var prevVisibleCells = unit.VisibleCells;

            unit.VisibleCells = GetVisibleCells(battlefield, toCell, visionRange);

            var unitRevealed = unit.VisibleCells.Except(prevVisibleCells);
            var unitHidden = prevVisibleCells.Except(unit.VisibleCells);

            cellsRevealed = unitRevealed.Except(unit.Owner.VisibleCells)
                .ToHashSet();

            unit.Owner.VisibleCells.UnionWith(cellsRevealed);

            cellsHidden = unitHidden.ToHashSet(); // TODO: ach this works for revealing cells but not hiding them.
            // Can't know with current setup if another unit can still see these.
            // Do we calculate a "vision except for this unit" here, or does our stored vision store the entities that can see each cell?

            // TODO: update unit.Owner.SeenCells
        }

        public void UpdateVisionForCreation(Battlefield battlefield, Unit unit)
        {
            int visionRange = unit.Definition.VisionRange;

            unit.VisibleCells = GetVisibleCells(battlefield, unit.Location, visionRange);
        }

        public void UpdateVisionForDestruction(Battlefield battlefield, Unit unit)
        {
            unit.Owner.CellsSeenThisTurn.UnionWith(unit.VisibleCells);
        }

        public HashSet<Cell> DetermineVision(Player player)
        {
            var currentVision = new HashSet<Cell>();

            foreach (var building in player.Buildings)
                currentVision.UnionWith(building.VisibleCells);

            foreach (var unit in player.Units)
                currentVision.UnionWith(unit.VisibleCells);

            return currentVision;
        }

        public void StartPlayerTurn(Player player)
        {
            player.CellsSeenThisTurn.Clear();

            // TODO: any more?
        }

        public void EndPlayerTurn(Player player)
        {
            // TODO: MORE!

            HideCells(player, player.CellsSeenThisTurn);

            player.CellsSeenThisTurn.Clear();
        }

        /*
        private void RevealCells(Player player, IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                if (!player.SeenCells.ContainsKey(cell))
                    player.SeenCells.Add(cell, null);
            }
        }
        */
        private void HideCells(Player player, IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                var building = cell.Building;

                player.SeenCells[cell] = building == null
                    ? null
                    : new BuildingSnapshot(building);
            }
        }
    }
}
