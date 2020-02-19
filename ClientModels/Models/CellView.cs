using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class CellView
    {
        public CellView(Cell cell, bool seen)
        {
            Type = cell.Type;

            Visibility = seen
                ? Visibility.Seen
                : Visibility.Unseen;

            if (seen && cell.Entity != null)
            {
                if (cell.Entity is Unit unit)
                    Content = new UnitView(unit);
                else if (cell.Entity is Building building)
                    Content = new BuildingView(building);
            }
        }

        public Visibility Visibility { get; }

        public CellType Type { get; }

        public EntityView Content { get; }
    }

    public enum Visibility
    {
        Unseen = 0,
        Seen = 1,
        Visible = 2,
    }
}
