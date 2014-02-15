//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TechTree.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Game
    {
        public Game()
        {
            this.GamePlayers = new HashSet<GamePlayer>();
            this.GameData = new HashSet<GameData>();
        }
    
        public int ID { get; set; }
        public GameStatus StatusID { get; set; }
        public Nullable<int> CurrentPlayerID { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime LastUpdated { get; set; }
    
        public virtual ICollection<GamePlayer> GamePlayers { get; set; }
        public virtual Player CurrentPlayer { get; set; }
        public virtual ICollection<GameData> GameData { get; set; }
    }
}