using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels
{
    public enum ObjectiveSubject
    {
        Units,
        ResourceReserves,
        RevealObjectives,
    }

    public class Objective
    {
        public Objective()
        {

        }

        public Objective(Objective copyFrom)
        {
            Description = copyFrom.Description;
            Value = copyFrom.Value;
            Subject = copyFrom.Subject;
            SubjectTypeID = copyFrom.SubjectTypeID;
            TargetQuantity = copyFrom.TargetQuantity;
            RelativeToOpponent = copyFrom.RelativeToOpponent;
            CellsByPlayer = new Dictionary<uint, int[]>(copyFrom.CellsByPlayer);
        }

        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }

        public ObjectiveSubject Subject { get; set; }

        public uint? SubjectTypeID { get; set; }

        public int TargetQuantity { get; set; }

        public bool RelativeToOpponent { get; set; }

        [JsonConverter(typeof(UintDictionaryConverter<int[]>))]
        public Dictionary<uint, int[]>? CellsByPlayer { get; set; }

        public bool IsSatisfied(Game game, Player player)
        {
            switch (Subject)
            {
                case ObjectiveSubject.Units:
                    return IsUnitQuantitySatisfied(game, player);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool IsUnitQuantitySatisfied(Game game, Player player)
        {
            Func<Unit, bool> unitFilter = unit => true;

            if (SubjectTypeID.HasValue)
            {
                unitFilter = unit => /*unitFilter(unit) &&*/ unit.Definition.ID == SubjectTypeID.Value;
            }

            if (CellsByPlayer != null)
            {
                unitFilter = unit => unitFilter(unit) && CellsByPlayer[unit.Owner.ID].Contains(unit.Location.ID);
            }

            int quantity = player.Units.Count(unitFilter);

            if (RelativeToOpponent)
            {
                int opponentQuantity = game.Players
                    .Where(opponent => opponent != player)
                    .Max(opponent => opponent.Units.Count(unitFilter));

                quantity -= opponentQuantity;
            }

            return quantity >= TargetQuantity;
        }
    }
}
