using ObjectiveStrategy.ClientModels.Models;
using ObjectiveStrategy.GameLogic.Services;
using ObjectiveStrategy.GameModels;

namespace ObjectiveStrategy.ClientModels.Factories
{
    class GameViewFactory
    {
        public GameViewFactory
        (
            VisionService visionService,
            PathingService pathingService
        )
        {
            PathingService = pathingService;
            VisionService = visionService;
        }

        public PathingService PathingService { get; }

        public VisionService VisionService { get; }

        public GameView CreateView(Game game, Player player)
        {
            return new GameView(game, player);
        }
    }
}
