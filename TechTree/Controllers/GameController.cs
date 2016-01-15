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
            var model = new GameModel() { Tree = tree };

            return View("Play", model);
        }
	}
}