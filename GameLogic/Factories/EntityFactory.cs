using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.GameLogic.Factories
{
    public class EntityFactory
    {
        public Building CreateBuilding(Game game, Player player, BuildingType type, Cell location)
        {
            return new Building(game.NextEntityID++, player, type, location);
        }

        public Unit CreateUnit(Game game, Building builtAt, UnitType type)
        {
            return new Unit(game.NextEntityID++, builtAt.Owner, type, builtAt.Location);
        }

        public Unit CreateUnit(Game game, Player player, UnitType type, Cell location)
        {
            return new Unit(game.NextEntityID++, player, type, location);
        }
    }
}
