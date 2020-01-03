using System.Collections.Generic;

namespace GameModels.Definitions
{
    public class BuildingType : EntityType, IBuildingType
    {
        public BuildingType(IBuildingType copyFrom)
            : base(copyFrom)
        {
            Unlocks = new List<uint>(copyFrom.Unlocks);

            Builds = new List<uint>(copyFrom.Builds);
            Researches = new List<uint>(copyFrom.Researches);

            DisplayRow = copyFrom.DisplayRow;
            DisplayColumn = copyFrom.DisplayColumn;
        }

        public List<uint> Unlocks { get; }

        public List<uint> Builds { get; }

        public List<uint> Researches { get; }


        public int DisplayRow { get; internal set; }

        public int DisplayColumn { get; internal set; }
    }
}
