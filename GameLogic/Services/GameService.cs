using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class GameService
    {
        public GameService(
            VisionService visionService,
            MovementService movementService,
            PathingService pathingService
        )
        {
            VisionService = visionService;

            MovementService = movementService;

            PathingService = pathingService;
        }

        private VisionService VisionService { get; }

        private MovementService MovementService { get; }

        private PathingService PathingService { get; }

        public void StartTurn(Game game)
        {
            StartPlayerTurn(game.CurrentPlayer);
        }

        private void StartPlayerTurn(Player player)
        {

            foreach (var unit in player.Units)
            {
                unit.MovementRemaining = unit.Definition.MoveRange;

                foreach (var feature in unit.Definition.Features)
                    feature.StartTurn(unit);
            }

            foreach (var building in player.Buildings)
                foreach (var feature in building.Definition.Features)
                    feature.StartTurn(building);

            // TODO: generate mana

            // TODO: tick any per-turn effects (which may involve removing them if they've expired)
        }

        public bool EndTurn(Game game)
        {
            EndPlayerTurn(game.CurrentPlayer);

            game.TurnsRemaining--;

            if (game.TurnsRemaining <= 0)
            {
                return false;
            }

            game.CurrentPlayerIndex = game.CurrentPlayerIndex >= game.Players.Length - 1
                ? 0
                : game.CurrentPlayerIndex + 1;

            return true;
        }

        private void EndPlayerTurn(Player player)
        {
            foreach (var unit in player.Units)
            {
                unit.MovementRemaining = unit.Definition.MoveRange;

                foreach (var feature in unit.Definition.Features)
                    feature.EndTurn(unit);
            }

            foreach (var building in player.Buildings)
                foreach (var feature in building.Definition.Features)
                    feature.EndTurn(building);
        }

        public bool TryMove(Game game, Unit unit, IList<int> cells)
        {
            if (unit.Owner != game.CurrentPlayer)
                return false;

            int[] moveCells = MovementService.TryMove(game.Battlefield, unit, cells);

            if (moveCells.Length == 0)
                return false;

            // TODO: fire "moved" event?
            return true;
        }
    }
}
