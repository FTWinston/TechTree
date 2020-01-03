using GameModels;
using GameModels.Definitions.Builders;
using GameModels.Instances;
using System;
using System.Web.Mvc;
using TechTree.Models;

namespace TechTree.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Game/Test
        public ActionResult Test()
        {
            var treeGenerator = new TreeGenerator();
            
            var model = new GameModel()
            {
                Tree = treeGenerator.Generate(),
                Map = GenerateRandomMap(),
            };

            return View("Play", model);
        }

        private static Map GenerateRandomMap()
        {
            var r = new Random();
            var map = new Map(37, 37);

            var halfSize = Math.Max(map.Width, map.Height) / 2.0f;
            var halfLower = (int)halfSize;
            var halfUpper = (int)(halfSize + 0.5f);

            for (var x = 0; x < map.Width; x++)
                for (var y = 0; y < map.Height; y++)
                    if (x + y >= halfLower && x + y < map.Width + map.Height - halfUpper)
                    {
                        Cell.CellType type;
                        switch (r.Next(5))
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

        //
        // GET: /Game/Tree/123
        public JsonResult Tree(int id)
        {
            var treeGenerator = new TreeGenerator(id, TreeGenerator.TreeComplexity.Normal);

            object data = treeGenerator.Generate();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}