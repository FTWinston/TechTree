﻿using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public interface IBuildingType : IEntityType
    {
        List<uint> Unlocks { get; }

        List<uint> Builds { get; }

        List<uint> Researches { get; }

        int DisplayRow { get; }

        int DisplayColumn { get; }
    }
}
