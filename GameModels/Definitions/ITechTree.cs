using System.Collections.Generic;

namespace GameModels.Definitions
{
    public interface ITechTree
    {
        Dictionary<uint, IBuildingType> Buildings { get; }

        Dictionary<uint, IUnitType> Units { get; }

        Dictionary<uint, Research> Research { get; }
    }
}
