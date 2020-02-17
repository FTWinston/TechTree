using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels
{
    public class Game
    {
        public Game(GameDefinition definition)
        {
            Battlefield = definition.Battlefield;

            Players = new List<Player>
            {
                new Player(1, new TechTree(definition.TechTree)),
                new Player(2, new TechTree(definition.TechTree))
            };

            TurnsRemaining = definition.TurnLimit * Players.Count;
        }

        public int TurnsRemaining { get; private set; }

        public Player CurrentPlayer { get; private set; }

        public bool FinishTurn()
        {
            TurnsRemaining--;

            if (TurnsRemaining <= 0)
            {
                return false;
            }

            int nextIndex = Players.IndexOf(CurrentPlayer);
            if (nextIndex >= Players.Count)
                nextIndex = 0;

            CurrentPlayer = Players[nextIndex];
            return true;
        }

        public List<Player> Players { get; }

        public Battlefield Battlefield { get; }
    }
}
