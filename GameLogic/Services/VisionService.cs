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

        private void RecordVisibleCells(Battlefield battlefield, Entity entity)
        {
            var cells = GetVisibleCells(battlefield, entity.Location, entity.BaseDefinition.VisionRange);

            foreach (var cell in cells)
                cell.EntitiesThatCanSee.Add(entity);

            entity.VisibleCells = cells;
        }

        private void RecordSnapshot(Entity entity, Cell cell)
        {
            entity.Owner.SeenCells[cell] = cell.Building == null || cell.Building == entity
                ? null
                : new BuildingSnapshot(cell.Building);
        }

        public bool CanSee(Player player, Cell cell)
        {
            return cell.EntitiesThatCanSee.Any(e => e.Owner == player);
        }

        public void PopulateVision(Game game)
        {
            foreach (var cell in game.Battlefield.Cells)
                if (cell != null)
                    cell.EntitiesThatCanSee.Clear();

            foreach (var player in game.Players)
            {
                foreach (var building in player.Buildings)
                    RecordVisibleCells(game.Battlefield, building);

                foreach (var unit in player.Units)
                    RecordVisibleCells(game.Battlefield, unit);
            }
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

            foreach (var cell in unitHidden)
                cell.EntitiesThatCanSee.Remove(unit);

            cellsRevealed = unitRevealed
                .Where(cell => !cell.EntitiesThatCanSee.Any(e => e.Owner == unit.Owner))
                .ToHashSet();

            cellsHidden = unitHidden
                .Where(cell => !cell.EntitiesThatCanSee.Any(e => e.Owner == unit.Owner))
                .ToHashSet();

            foreach (var cell in unitRevealed)
                cell.EntitiesThatCanSee.Add(unit);

            foreach (var cell in cellsHidden)
                RecordSnapshot(unit, cell);
        }

        public void UpdateVisionForCreation(Battlefield battlefield, Entity entity, out HashSet<Cell> cellsRevealed)
        {
            int visionRange = entity.BaseDefinition.VisionRange;

            entity.VisibleCells = GetVisibleCells(battlefield, entity.Location, visionRange);

            cellsRevealed = new HashSet<Cell>();

            foreach (var cell in entity.VisibleCells)
            {
                entity.Owner.SeenCells.Remove(cell);

                if (!CanSee(entity.Owner, cell))
                {
                    cellsRevealed.Add(cell);
                    entity.Owner.SeenCells.Remove(cell);
                }

                cell.EntitiesThatCanSee.Add(entity);
            }
        }

        public void UpdateVisionForDestruction(Battlefield battlefield, Entity entity, out HashSet<Cell> cellsHidden)
        {
            cellsHidden = new HashSet<Cell>();

            foreach (var cell in entity.VisibleCells)
            {
                cell.EntitiesThatCanSee.Remove(entity);

                if (!CanSee(entity.Owner, cell))
                {
                    cellsHidden.Add(cell);
                    RecordSnapshot(entity, cell);
                }
            }
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
    }
}
