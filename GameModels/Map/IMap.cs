using System;
using System.Collections.Generic;

namespace GameModels.Map
{
    public interface IMap<TCell>
    {
        IEnumerable<TCell> GetNeighbors(TCell from);

        HashSet<TCell> GetCellsWithinDistance(TCell from, int distance);

        int GetDistance(TCell from, TCell to);

        IEnumerable<TCell> TraceLine(TCell from, TCell to);
    }
    
    /*
    public class VisionService
    {
        public HashSet<TCell> GetVisibleCells<TCell>(IMap<TCell> map, TCell from, int range, Func<TCell, bool> blocksVision)
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
    */
}
