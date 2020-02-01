using GameModels.Definitions;
using System.Collections.Generic;

namespace GameModels
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
        }

        public List<Player> Players { get; }

        public Battlefield Battlefield { get; }
    }
}
