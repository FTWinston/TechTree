using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using System.Linq;

namespace ObjectiveStrategy.GameLogic.Factories
{
    public class GameFactory
    {
        public Game CreateGame(GameDefinition definition)
        {
            var battlefield = new Battlefield(definition.Battlefield);

            var players = new Player[]
            {
                new Player(1, new TechTree(definition.TechTree)),
                new Player(2, new TechTree(definition.TechTree))
            };

            int duration = definition.TurnLimit * players.Length;

            var objectives = definition.Objectives
                .Select(o => new Objective(o))
                .ToArray();

            return new Game(battlefield, objectives, players, duration);
        }
    }
}
