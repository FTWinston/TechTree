using Newtonsoft.Json;

namespace ObjectiveStrategy.GameModels
{
    public class Game
    {
        public Game(Battlefield battlefield, Objective[] objectives, Player[] players, int turnsRemaining)
        {
            Battlefield = battlefield;

            Objectives = objectives;

            Players = players;

            TurnsRemaining = turnsRemaining;
        }

        public uint NextEntityID { get; set; }

        public int TurnsRemaining { get; set; }

        public Battlefield Battlefield { get; }

        public Objective[] Objectives { get; }

        public Player[] Players { get; }

        [JsonProperty(PropertyName = "CurrentPlayer")]
        public int CurrentPlayerIndex { get; set; } = 0;

        [JsonIgnore]
        public Player CurrentPlayer => Players[CurrentPlayerIndex];
    }
}
