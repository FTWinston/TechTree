using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechTree.Models
{
    public class HomePageModel
    {
        public List<Game> CurrentGamesMyTurn = new List<Game>(), CurrentGamesNotMyTurn = new List<Game>();
        public bool HasAnyGames { get { return CurrentGamesMyTurn.Count > 0 || CurrentGamesNotMyTurn.Count > 0; } }
    }
}