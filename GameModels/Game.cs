using Newtonsoft.Json;
using ObjectiveStrategy.GameModels.Events;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;

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

        public event EventHandler<MoveEvent>? UnitMoved;

        public void OnUnitMoved(Unit unit, Cell from, Cell to, HashSet<Cell> cellsRevealed, HashSet<Cell> cellsHidden)
        {
            UnitMoved?.Invoke(this, new MoveEvent(unit, from, to, cellsRevealed, cellsHidden));
        }
    }
}
