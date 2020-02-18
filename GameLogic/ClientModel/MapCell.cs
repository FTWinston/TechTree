using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.GameLogic.ClientModel
{
    public struct MapCell
    {
        public MapCell(Cell cell, bool seen)
        {
            Type = cell.Type;

            Visibility = seen
                ? Visibility.Seen
                : Visibility.Unseen;

            Content = seen
                ? cell.Entity
                : null;
        }

        public Visibility Visibility { get; }

        public CellType Type { get; }

        public Entity Content { get; }
    }

    public enum Visibility
    {
        Unseen = 0,
        Seen = 1,
        Visible = 2,
    }
}
