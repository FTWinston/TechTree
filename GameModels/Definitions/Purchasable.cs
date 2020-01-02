using System.Collections.Generic;

namespace GameModels.Definitions
{
    public abstract class Purchasable : IPurchasable
    {
        protected Purchasable(IPurchasable copyFrom)
        {
            Name = copyFrom.Name;
            Symbol = copyFrom.Symbol;

            BuildTime = copyFrom.BuildTime;
            Cost = new Dictionary<ResourceType, int>(copyFrom.Cost);

            Prerequisite = copyFrom.Prerequisite;
        }

        public string Name { get; }

        public string Symbol { get; }

        public int BuildTime { get; internal set; }

        public Dictionary<ResourceType, int> Cost { get; internal set; }

        public uint? Prerequisite { get; internal set; }
    }
}
