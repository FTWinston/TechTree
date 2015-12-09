using GameModels.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechTree.Controllers
{
    public class TechTreeController : Controller
    {
        //
        // GET: /TechTree/Random
        public ActionResult Random(TreeGenerator.Complexity? complexity = null)
        {
            if (!complexity.HasValue)
                complexity = TreeGenerator.Complexity.Normal;

            var tree = TreeGenerator.Generate(complexity.Value);
            return View("View", tree);
        }

        //
        // GET: /TechTree/Generate?seed=<seed>
        public ActionResult Generate(int seed, TreeGenerator.Complexity? complexity = null)
        {
            if (!complexity.HasValue)
                complexity = TreeGenerator.Complexity.Normal;

            var tree = TreeGenerator.Generate(complexity.Value, seed);
            return View("View", tree);
        }
	}
}