using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public abstract class BaseTechTree
    {
        internal abstract IEnumerable<KeyValuePair<uint, IBuildingType>> AllBuildings { get; }

        internal abstract IEnumerable<KeyValuePair<uint, IUnitType>> AllUnits { get; }

        public abstract Dictionary<uint, Research> Research { get; set; }
    }

    public abstract class BaseTechTree<TBuildingType, TUnitType> : BaseTechTree
        where TBuildingType : IBuildingType
        where TUnitType : IUnitType
    {
        protected BaseTechTree() : this
        (
            new Dictionary<uint, TBuildingType>(),
            new Dictionary<uint, TUnitType>(),
            new Dictionary<uint, Research>()
        ) { }

        protected BaseTechTree
        (
            Dictionary<uint, TBuildingType> buildings,
            Dictionary<uint, TUnitType> units,
            Dictionary<uint, Research> research
        )
        {
            Buildings = buildings;
            Units = units;
            Research = research;
        }

        public Dictionary<uint, TBuildingType> Buildings { get; set; }

        public Dictionary<uint, TUnitType> Units { get; set; }

        public override Dictionary<uint, Research> Research { get; set; }

        internal override IEnumerable<KeyValuePair<uint, IBuildingType>> AllBuildings =>
            Buildings.Select(kvp => new KeyValuePair<uint, IBuildingType>(kvp.Key, kvp.Value));

        internal override IEnumerable<KeyValuePair<uint, IUnitType>> AllUnits =>
            Units.Select(kvp => new KeyValuePair<uint, IUnitType>(kvp.Key, kvp.Value));
    }
}
