﻿using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public interface IPurchasable
    {
        string Name { get; }

        string Symbol { get; }

        int BuildTime { get; }

        Dictionary<ResourceType, int> Cost { get; }

        uint? Prerequisite { get; }
    }
}
