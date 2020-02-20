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

            Battlefield = new BattlefieldView(Player, Game.Battlefield);
        }

        private Game Game { get; }

        private Player Player { get; }

        public int LocalPlayerID => Player.ID;

        public BattlefieldView Battlefield { get; }

        public TechTree TechTree => Player.TechTree;
    }
}
