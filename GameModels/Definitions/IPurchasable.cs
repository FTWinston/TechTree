using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public interface IPurchasable : ISelectable
    {
        int BuildTime { get; }

        Dictionary<ResourceType, int> Cost { get; }

        uint? Prerequisite { get; }
    }
}
