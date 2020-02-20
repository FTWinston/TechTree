using ObjectiveStrategy.ClientModels.Models;
using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

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
        public GameView Generate()
        {
            var gameDef = GameGenerator.GameGenerator.GenerateGame();
            var game = new Game(gameDef);
            return new GameView(game, game.Players.First());
        }

        [HttpGet("[action]/{seed}")]
        public GameView Generate(int complexity, int seed)
        {
            var gameDef = GameGenerator.GameGenerator.GenerateGame(complexity, seed);
            var game = new Game(gameDef);
            return new GameView(game, game.Players.First());
        }
    }
}
