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
            ViewBag.Message = "Welcome to Tech Tree. This is very much a work in progress. Tread softly.";

            var model = new HomePageModel();
            var myGames = new List<Game>();
            Player player;

            using (GameContext db = new GameContext())
            {
                player = GetPlayer(db);
                myGames = db.Games
                    .Where(g => g.StatusID != GameStatus.Finished && g.StatusID != GameStatus.PublicSetup && g.GamePlayers.Where(gp => gp.PlayerID == player.ID).Count() > 0)
                    .OrderBy(g => g.ID)
                    .ToList();

                model.SearchingForGame = db.Games.Where(g => g.StatusID == GameStatus.PublicSetup && g.GamePlayers.Where(gp => gp.PlayerID == player.ID).Count() > 0).Count() > 0;
                model.GameModes = db.GameModes.ToList();
            }

            foreach (var game in myGames)
                if (game.CurrentPlayerID == player.ID)
                    model.CurrentGamesMyTurn.Add(game);
                else
                    model.CurrentGamesNotMyTurn.Add(game);

            return View(model);
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
                    Player player = GetPlayer(db, expression);
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
                player = GetPlayer(db, name);

            if (player == null)
                return RedirectToAction("Index");

            return View(player);
        }

        private Player GetPlayer(GameContext db, string name = null)
        {
            if ( name == null)
                name = User.Identity.Name;

            return db.Players.FirstOrDefault(p => p.Name.ToLower() == name);
        }
    }
}
