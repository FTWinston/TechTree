using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Map
{
    public interface IMap<TCell>
    {
        IEnumerable<TCell> GetNeighbors(TCell from);

        HashSet<TCell> GetCellsWithinDistance(TCell from, int distance);

        int GetDistance(TCell from, TCell to);

        IEnumerable<TCell> TraceLine(TCell from, TCell to);
    }
}
