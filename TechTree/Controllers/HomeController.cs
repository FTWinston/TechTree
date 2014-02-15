using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechTree.Models;

namespace TechTree.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            using (GameContext db = new GameContext())
            {
                Player player = db.Players.FirstOrDefault(p => p.Name.ToLower() == User.Identity.Name);
                ViewBag.playerID = player.ID;
                ViewBag.games = db.Games.Where(g => g.GamePlayers.Where(gp => gp.PlayerID == player.ID).Count() > 0).ToList();
            }

            return View();
        }

        public ActionResult News()
        {
            ViewBag.Message = "This is the news page.";

            return View();
        }

        public ActionResult Search()
        {
            //ViewBag.Message = "Search for players and games";

            return View();
        }

        [HttpPost]
        public ActionResult Search(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                ModelState.AddModelError("expression", "Please enter a value.");
                return View();
            }

            expression = expression.Trim().ToLower();

            using (GameContext db = new GameContext())
            {
                int gameID;
                if (int.TryParse(expression, out gameID) && gameID > 0)
                {
                    // if numeric, must be a game
                    Game game = db.Games.FirstOrDefault(g => g.ID == gameID);
                    if (game == null)
                    {
                        ModelState.AddModelError("expression", "No game found with this ID.");
                        return View();
                    }

                    return RedirectToAction("Index", "Game", new { id = game.ID });
                }
                else
                {
                    // if non-numeric, must be a player
                    Player player = db.Players.FirstOrDefault(p => p.Name.ToLower() == expression);
                    if (player == null)
                    {
                        ModelState.AddModelError("expression", "No player found with this name.");
                        return View();
                    }

                    return RedirectToAction("Profile", new { name = player.Name });
                }
            }
        }

        public new ActionResult Profile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = User.Identity.Name;
            name = name.ToLower();

            Player player;
            using (GameContext db = new GameContext()) 
                player = db.Players.FirstOrDefault(p => p.Name.ToLower() == name);

            if (player == null)
                return RedirectToAction("Index");

            ViewBag.Name = player.Name;
            ViewBag.Wins = player.Wins;
            ViewBag.Losses = player.Losses;
            
            return View();
        }
    }
}
