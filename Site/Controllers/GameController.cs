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
            Random random = new Random();
            var map = new Battlefield(37, 37);

            var halfSize = Math.Max(map.Width, map.Height) / 2.0f;
            var halfLower = (int)halfSize;
            var halfUpper = (int)(halfSize + 0.5f);

            for (var x = 0; x < map.Width; x++)
                for (var y = 0; y < map.Height; y++)
                    if (x + y >= halfLower && x + y < map.Width + map.Height - halfUpper)
                    {
                        Cell.CellType type;
                        switch (random.Next(8))
                        {
                            case 1:
                                type = Cell.CellType.Difficult; break;
                            case 2:
                                type = Cell.CellType.Unpassable; break;
                            default:
                                type = Cell.CellType.Flat; break;
                        }
                        map.Cells[x + y * map.Width] = new Cell(y, x, type);
                    }

            return map;
        }
    }
}
