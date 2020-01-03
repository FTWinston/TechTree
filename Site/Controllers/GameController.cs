using GameModels.Definitions.Builders;
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
        public GameModels.TechTree Tree()
        {
            var treeGenerator = new TreeGenerator();
            return treeGenerator.Generate();
        }

        [HttpGet("[action]/{seed}")]
        public GameModels.TechTree Tree(int seed)
        {
            var treeGenerator = new TreeGenerator(seed);
            return treeGenerator.Generate();
        }
    }
}
