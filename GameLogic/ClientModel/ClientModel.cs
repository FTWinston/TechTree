using ObjectiveStrategy.GameModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameLogic.ClientModel
{
    public class ClientModel
    {
        public ClientModel(Game game, Player player)
        {
            if (!game.Players.Contains(player))
            {
                throw new InvalidOperationException("Cannot generate a view model for a player that isn't in the game");
            }

            Game = game;
            Player = player;
        }

        private Game Game { get; }

        private Player Player { get; }

        public int Width => Game.Battlefield.Width;

        public int Height => Game.Battlefield.Height;

        public MapCell[] Cells => Game.Battlefield.Cells
            .Select(cell => new MapCell(cell, Player.SeenCells.Contains(cell)))
            .ToArray();

        public TechTree TechTree => Player.TechTree;

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
