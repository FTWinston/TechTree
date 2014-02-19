using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechTree.Models;

namespace TechTree.Controllers
{
    public class SetupController : Controller
    {
        //
        // GET: /Setup/

        public ActionResult Index()
        {
            return View();
        }

        private Player GetPlayer(GameContext db, string name = null)
        {
            if (name == null)
                name = User.Identity.Name;

            return db.Players.FirstOrDefault(p => p.Name.ToLower() == name);
        }

        [HttpPost]
        public ActionResult FindGame(List<int> modes)
        {
            if (modes == null || modes.Count == 0)
                return RedirectToAction("Index", "Home");

            using (GameContext db = new GameContext())
            {
                var player = GetPlayer(db);
                var game = db.Games.Where(g => g.StatusID == GameStatus.PublicSetup && modes.Contains(g.GameModeID)).FirstOrDefault();
                bool ready = false;

                if (game == null)
                {
                    var gameModes = db.GameModes.Where(m => modes.Contains(m.ID)).ToList();
                    if (gameModes.Count == 0)
                        return RedirectToAction("Index", "Home");

                    Random r = new Random();

                    game = new Game();
                    game.StatusID = GameStatus.PublicSetup;
                    game.CreatedOn = game.LastUpdated = DateTime.Now;
                    game.GameMode = gameModes[r.Next(gameModes.Count)];
                    db.Games.Add(game);

                    int numPlayers = r.Next(game.GameMode.MinPlayers, game.GameMode.MaxPlayers + 1);
                    for (int i = 1; i <= numPlayers; i++)
                    {
                        GamePlayer gp = new GamePlayer();
                        gp.Active = true;
                        gp.Number = i;
                        gp.Player = i == 1 ? player : null;
                        game.GamePlayers.Add(gp);
                    }
                }
                else
                {
                    var emptySlot = game.GamePlayers.Where(p => p.Player == null).OrderByDescending(p => p.Number).FirstOrDefault();
                    emptySlot.Player = player;

                    if (game.GamePlayers.Where(p => p.Player == null).Count() == 0)
                    {
                        game.StatusID = GameStatus.InProgress;
                        ready = true;
                    }
                }
                db.SaveChanges();

                // if game ready to start, go straight to game. Otherwise, go to index.
                if (ready)
                    return RedirectToAction("Index", "Game", new { id = game.ID });
                else
                    return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult HostGame()
        {
            throw new NotImplementedException();
        }
    }
}
