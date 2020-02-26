using ObjectiveStrategy.GameModels;

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
            // TODO: update construction and researching

            foreach (var unit in player.Units)
            {
                unit.MovementRemaining = unit.Definition.MoveRange;
            }

            // TODO: reset all attacks (or other "per turn" abilities ... this is done by feature, i guess)

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
            // TODO: have resource buildings give resources
        }
    }
}
