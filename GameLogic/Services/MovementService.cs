using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class MovementService
    {
        public MovementService(VisionService visionService)
        {
            VisionService = visionService;
        }

        private VisionService VisionService { get; }

        public bool Remove(Unit unit)
        {
            return unit.Location.Units.Remove(unit);
        }

        public bool Place(Unit unit, Cell location)
        {
            unit.Location = location;

            if (!location.Units.Contains(unit))
                location.Units.Add(unit);

            return true;
        }

        public int[] TryMove(Battlefield battlefield, Unit unit, IList<int> desiredCells)
        {
            var moveCells = DetermineMovePath(battlefield, unit, desiredCells);

            if (moveCells.Count == 0)
                return new int[] { };

            Cell startCell = unit.Location;
            Remove(unit);
            Place(unit, moveCells.Last());
            
            unit.MovementRemaining -= moveCells.Count;

            VisionService.UpdateVisionForMove(battlefield, unit, startCell, moveCells);

            return moveCells
                .Select(cell => cell.ID)
                .Prepend(startCell.ID)
                .ToArray();
        }
        private List<Cell> DetermineMovePath(Battlefield battlefield, Unit unit, IList<int> desiredCells)
        {
            int movementRemaining = unit.MovementRemaining;
            var moveCells = new List<Cell>();

            Cell currentCell = unit.Location;
            foreach (int nextCellID in desiredCells)
            {
                Cell? nextCell = battlefield.GetNeighbors(currentCell)
                    .FirstOrDefault(cell => cell.ID == nextCellID);

                if (nextCell == null)
                    return moveCells; // not an adjacent cell, so give up

                int moveStepCost = GetMoveStepCost(unit, currentCell, nextCell);

                if (movementRemaining < moveStepCost)
                    return moveCells; // ran out of movement, so give up

                movementRemaining -= moveStepCost;

                if (!CanLeaveCell(unit, currentCell) || !CanEnterCell(unit, nextCell))
                {
                    return moveCells; // can't make the move (e.g. due to terrain type?)
                }

                moveCells.Add(nextCell);
                currentCell = nextCell;
            }

            return moveCells;
        }

        private int GetMoveStepCost(Unit unit, Cell fromCell, Cell toCell)
        {
            // TODO: account for rough terrain?
            return 1;
        }

        private bool CanLeaveCell(Unit unit, Cell cell)
        {
            // TODO: when might this not be true? e.g. if a unit gets immobilised part way through a move
            return true;
        }

        private bool CanEnterCell(Unit unit, Cell cell)
        {
            // Cannot move into an occupied cell.
            // TODO: if we run into an invisible unit, it ought to be (partly?) revealed.
            return cell.Building == null && cell.Units.Count == 0;
        }
    }
}
