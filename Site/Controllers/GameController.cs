using ObjectiveStrategy.ClientModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using ObjectiveStrategy.GameGeneration;
using ObjectiveStrategy.GameLogic.Factories;

namespace ObjectiveStrategy.Site.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private ILogger<GameController> Logger { get; }

        public GameController(ILogger<GameController> logger)
        {
            Logger = logger;
        }

        [HttpGet("[action]")]
        public GameView Generate([FromServices] GameFactory gameFactory)
        {
            var definitionFactory = new GameDefinitionFactory(); 
            var gameDef = definitionFactory.GenerateGame();
            var game = gameFactory.CreateGame(gameDef);
            return new GameView(game, game.Players.First());
        }

        [HttpGet("[action]/{seed}")]
        public GameView Generate([FromServices] GameFactory gameFactory, int complexity, int seed)
        {
            var definitionFactory = new GameDefinitionFactory();
            var gameDef = definitionFactory.GenerateGame(complexity, seed);
            var game = gameFactory.CreateGame(gameDef);
            return new GameView(game, game.Players.First());
        }
    }
}
