using ObjectiveStrategy.GameModels.Map;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class VisionService
    {
        public HashSet<TCell> GetVisibleCells<TCell>(IGraph<TCell> map, TCell from, int range, Func<TCell, bool> blocksVision)
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
    }
}
