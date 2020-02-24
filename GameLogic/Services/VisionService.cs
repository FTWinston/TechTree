using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Map;
using System;
using System.Collections.Generic;

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

        public HashSet<Cell> GetVisibleCells(IGraph<Cell> map, Cell from, int range)
        {
            return GetVisibleCells(map, from, range, cell => cell.BlocksVision);
        }

        public void RevealCells(Player player, IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                player.SeenCells.Remove(cell);
            }
        }

        public void HideCells(Player player, IEnumerable<Cell> cells)
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
