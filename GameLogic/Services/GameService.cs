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
    }
}
