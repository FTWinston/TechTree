using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System.Linq;

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

                if (cell.Building != null)
                {
                    Building = new BuildingView(player, cell.Building);
                }

                if (cell.Units.Count > 0)
                {
                    Units = cell.Units
                        .Select(u => new UnitView(player, u))
                        .ToArray();
                }
            }
            else if (player.SeenCells.TryGetValue(cell, out var snapshot))
            {
                Visibility = Visibility.Seen;

                if (snapshot != null)
                    Building = new BuildingSnapshotView(snapshot);
            }
            else
            {
                Visibility = Visibility.Unseen;
            }
        }

        public Visibility Visibility { get; }

        public CellType Type { get; }

        //[JsonIgnore]
        public EntityView? Building { get; }

        // This would be needed if EntityView doesn't expose every building property we care about (e.g. builds-in-progress etc)
        //[JsonPropertyName("Building")]
        //public object? RawBuilding => Building;

        public UnitView[]? Units { get; }
    }

    public enum Visibility
    {
        Unseen = 0,
        Seen = 1,
        Visible = 2,
    }
}
