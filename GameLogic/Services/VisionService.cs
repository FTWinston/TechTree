using GameModels;
using GameModels.Instances;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Services
{
    public class VisionService
    {
        public HashSet<Cell> GetVisibleCells(Battlefield battlefield, Cell startLocation, int range)
        {
            var results = new HashSet<Cell> { startLocation };

            AddVisibleCells(results, battlefield, startLocation, range);

            return results;
        }

        private void AddVisibleCells(HashSet<Cell> results, Battlefield battlefield, Cell from, int range)
        {
            var newNeighbours = battlefield
                .GetNeighbours(from)
                .Where(neighbour => !results.Contains(neighbour)) // "superfluous" check here so we can avoid recursively calling into visited cells
                .ToArray();

            results.UnionWith(newNeighbours);

            if (range > 1)
            {
                range--;

                foreach (var neighbour in newNeighbours)
                {
                    if (!neighbour.BlocksVision)
                    {
                        AddVisibleCells(results, battlefield, neighbour, range - 1);
                    }
                }
            }
        }
    }
}
