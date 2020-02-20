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

        //[JsonIgnore]
        public EntityView? Content { get; }

        //[JsonPropertyName("Content")]
        //public object? RawContent { get; } // needs to be object if EntityView doesn't have every property we might need
    }

    public enum Visibility
    {
        Unseen = 0,
        Seen = 1,
        Visible = 2,
    }
}
