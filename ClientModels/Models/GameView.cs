using ObjectiveStrategy.GameModels;
using System;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class GameView
    {
        public GameView(Game game, Player player)
        {
            if (!game.Players.Contains(player))
            {
                throw new InvalidOperationException("Cannot generate a view model for a player that isn't in the game");
            }

            Game = game;

            Player = player;

            BattlefieldView = new BattlefieldView(Player, Game.Battlefield);
        }

        private Game Game { get; }

        private Player Player { get; }

        public int PlayerID => Player.ID;

        public BattlefieldView BattlefieldView { get; }

        public TechTree TechTree => Player.TechTree;
    }
}
