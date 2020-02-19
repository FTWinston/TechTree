using ObjectiveStrategy.GameModels;
using System;
using System.Linq;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class BattlefieldView
    {
        public BattlefieldView(Player player, Battlefield battlefield)
        {
            Player = player;
            Battlefield = battlefield;
        }

        private Player Player { get; }
        private Battlefield Battlefield { get; }

        public int Width => Battlefield.Width;

        public int Height => Battlefield.Height;

        public CellView[] Cells => Battlefield.Cells
            .Select(cell => new CellView(cell, Player.SeenCells.Contains(cell)))
            .ToArray();

        /*
        private Dictionary<Entity, HashSet<Cell>> VisionByEntity { get; } = new Dictionary<Entity, HashSet<Cell>>();

        public HashSet<int> VisibleCells
        {
            get
            {
                var cells = new HashSet<int>();

                var service = new VisionService();

                foreach (var entity in VisionByEntity)
                {
                    service.GetVisibleCells(Game.Battlefield, entity.Key.Location, entity.Key.VisionRange, )
                }

                foreach (var building in Player.Buildings)
                {

                    // building.Location.Index
                }

                return cells;
            }
        }
        */
    }
}
