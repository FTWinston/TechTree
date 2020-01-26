using GameModels;
using GameModels.Instances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TreeGeneration;

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
        public TechTree Tree()
        {
            var treeGenerator = new TreeGenerator();
            return treeGenerator.Generate();
        }

        [HttpGet("[action]/{seed}")]
        public TechTree Tree(int seed)
        {
            var treeGenerator = new TreeGenerator(seed);
            return treeGenerator.Generate();
        }

        [HttpGet("[action]")]
        public Battlefield Battlefield()
        {
            var generator = new BattlefieldGenerator();
            return generator.Generate();
        }
    }
}
