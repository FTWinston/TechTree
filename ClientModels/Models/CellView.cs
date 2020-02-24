using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class CellView
    {
        public CellView(Cell cell, Player player)
        {
            Type = cell.Type;

            bool canSee = false; // TODO: determine this

            if (canSee)
            {
                Visibility = Visibility.Visible;

                if (cell.Entity != null)
                {
                    if (cell.Entity is Unit unit)
                        Content = new UnitView(player, unit);
                    else if (cell.Entity is Building building)
                        Content = new BuildingView(player, building);
                }
            }
            else if (player.SeenCells.TryGetValue(cell, out var snapshot))
            {
                Visibility = Visibility.Seen;

                if (snapshot != null)
                    Content = new BuildingSnapshotView(snapshot);
            }
            else
            {
                Visibility = Visibility.Unseen;
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
