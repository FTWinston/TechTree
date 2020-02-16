using GameModels;
using GameModels.Instances;
using System.Collections.Generic;

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
            foreach (var neighbour in battlefield.GetNeighbours(from))
            {
                results.Add(neighbour);

                if (!neighbour.BlocksVision && range > 1)
                {
                    AddVisibleCells(results, battlefield, neighbour, range - 1);
                }
            }
        }
    }
}
