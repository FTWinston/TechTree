using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechTree.Models
{
    public class HomePageModel
    {
        public List<Game> CurrentGamesMyTurn = new List<Game>(), CurrentGamesNotMyTurn = new List<Game>();
        public bool HasAnyGames { get { return CurrentGamesMyTurn.Count > 0 || CurrentGamesNotMyTurn.Count > 0; } }
        public bool SearchingForGame { get; set; }

        public List<GameMode> GameModes = new List<GameMode>();
        // recent news item(s)?
        // online players
        // recent games
        // ...and its not needed in the model, but a link to help documentation
    }
}