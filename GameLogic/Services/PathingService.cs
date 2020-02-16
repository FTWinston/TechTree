using GameModels;
using GameModels.Instances;
using System.Collections.Generic;

namespace GameLogic.Services
{
    public class PathingService
    {
        public HashSet<Cell> GetReachableCells(Battlefield battlefield, Cell from, int maxDistance)
        {
            var visited = new HashSet<Cell> { from };

            var fringes = new List<List<Cell>>
            {
                new List<Cell> { from },
            };

            for (var k = 1; k < maxDistance; k++)
            {
                var fringe = new List<Cell>();
                fringes.Add(fringe);

                foreach (var test in fringes[k - 1])
                    foreach (var neighbour in battlefield.GetNeighbours(from))
                    {
                        if (!neighbour.IsPassable || visited.Contains(neighbour))
                            continue;

                        visited.Add(neighbour);
                        fringe.Add(neighbour);
                    }
            }

            return visited;
        }
    }
}
