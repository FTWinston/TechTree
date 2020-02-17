using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Map;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class PathingService
    {
        public HashSet<TCell> GetReachableCells<TCell>(IGraph<TCell> graph, TCell from, int maxDistance, Func<TCell, bool> isPassable)
        {
            var visited = new HashSet<TCell> { from };

            var fringes = new List<List<TCell>>
            {
                new List<TCell> { from },
            };

            for (var k = 1; k < maxDistance; k++)
            {
                var fringe = new List<TCell>();
                fringes.Add(fringe);

                foreach (var test in fringes[k - 1])
                    foreach (var neighbour in graph.GetNeighbors(from))
                    {
                        if (!isPassable(neighbour) || visited.Contains(neighbour))
                            continue;

                        visited.Add(neighbour);
                        fringe.Add(neighbour);
                    }
            }

            return visited;
        }
    }
}
