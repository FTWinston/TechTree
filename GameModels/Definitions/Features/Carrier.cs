namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Carrier : PassiveFeature
    {
        public Carrier(int capacity)
        {
            Capacity = capacity;
        }

        public override string Name => "Carrier";

        public override string Description => $"Carries up to {Capacity} units";

        public override string Symbol => "⚖";

        private int Capacity { get; }
    }
}
