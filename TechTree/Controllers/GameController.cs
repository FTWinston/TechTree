using GameModels;
using GameModels.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            var tree = TreeGenerator.Generate(TreeGenerator.Complexity.Normal);
            var map = new Map(15, 15);
            var model = new GameModel() { Tree = tree, Map = map };
            
            for (var x = 0; x < map.Width; x++)
                for (var y = 0; y < map.Height; y++)
                    if (x + y > 6 && x + y <= map.Width + map.Height - 9)
                        map.Cells[x + y * map.Width] = new GameModels.Instances.Cell(y, x, GameModels.Instances.Cell.CellType.Flat);

            return View("Play", model);
        }

        //
        // GET: /Game/Tree/123
        public JsonResult Tree(int id)
        {
            var tree = TreeGenerator.Generate(TreeGenerator.Complexity.Normal);

            object data = tree;

            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}