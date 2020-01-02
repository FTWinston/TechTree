using System;
using System.Collections.Generic;

namespace GameModels.Definitions.Builders
{
    public class BuildingBuilder : EntityBuilder, IBuildingType
    {
        public BuildingBuilder(Random random, string symbol, BuildingRole role)
            : base(random, symbol)
        {
            Role = role;
        }

        public BuildingRole Role { get; }


        public List<uint> Unlocks { get; } = new List<uint>();

        public List<uint> UpgradesTo { get; set; }

        public uint UpgradesFrom { get; set; }


        public List<uint> Builds { get; set; }

        public List<uint> Researches { get; set; }


        public int DisplayRow { get; set; }

        public int DisplayColumn { get; set; }


        public BuildingType Build()
        {
            return new BuildingType(this);
        }

        public enum BuildingRole
        {
            Resource,
            Factory,
            Research,
            Defense,
            Utility,
        }
    }
}
