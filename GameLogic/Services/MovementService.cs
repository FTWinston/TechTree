using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Map;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class MovementService
    {
        public bool Remove(Unit unit)
        {
            return unit.Location.Units.Remove(unit);
        }

        public bool Place(Unit unit, Cell location)
        {
            if (location.Building != null || location.Units.Count > 0)
                return false;

            unit.Location = location;

            if (!location.Units.Contains(unit))
                location.Units.Add(unit);

            return true;
        }

        public bool Move(Unit unit, Cell destination)
        {
            var prevLocation = unit.Location;

            if (!Place(unit, destination))
                return false;

            prevLocation.Units.Remove(unit);
            return true;
        }
    }
}
