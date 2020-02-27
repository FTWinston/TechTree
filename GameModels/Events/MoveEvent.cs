using ObjectiveStrategy.GameModels.Instances;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Events
{
    public class MoveEvent
    {
        public MoveEvent(Unit unit, Cell from, Cell to, HashSet<Cell> cellsRevealed, HashSet<Cell> cellsHidden)
        {
            Unit = unit;
            FromCell = from;
            ToCell = to;
            Revealed = cellsRevealed;
            Hidden = cellsHidden;
        }

        public Unit Unit { get; }

        public Cell FromCell { get; }

        public Cell ToCell { get; }

        public HashSet<Cell> Revealed { get; }

        public HashSet<Cell> Hidden { get; }
    }
}
