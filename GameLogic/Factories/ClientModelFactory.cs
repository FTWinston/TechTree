using ObjectiveStrategy.GameLogic.Services;
using ObjectiveStrategy.GameModels;

namespace ObjectiveStrategy.GameLogic.Factories
{
    class ClientModelFactory
    {
        public ClientModelFactory
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

        public ClientModel.ClientModel GetClientModel(Game game, Player player)
        {
            return new ClientModel.ClientModel(game, player);
        }
    }
}
