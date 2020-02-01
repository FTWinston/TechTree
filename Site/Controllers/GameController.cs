using GameModels.Definitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Site.Controllers
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
        public GameDefinition Generate()
        {
            return GameGenerator.GameGenerator.GenerateGame();
        }

        [HttpGet("[action]/{seed}")]
        public GameDefinition Generate(int seed)
        {
            return GameGenerator.GameGenerator.GenerateGame(seed);
        }
    }
}
