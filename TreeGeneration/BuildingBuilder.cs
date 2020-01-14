using GameModels.Definitions;
using System;
using System.Collections.Generic;

namespace TreeGeneration
{
    public class BuildingBuilder : EntityBuilder, IBuildingType
    {
        public BuildingBuilder(Random random, string symbol, BuildingRole role)
            : base(random, symbol)
        {
            Role = role;
            Name = "Unnamed building";
        }

        public BuildingRole Role { get; }


        public List<uint> Unlocks { get; } = new List<uint>();

        public List<uint> Builds { get; } = new List<uint>();

        public List<uint> Researches { get; } = new List<uint>();


        public int DisplayRow { get; set; }

        public int DisplayColumn { get; set; }

        public override void AllocateName(ICollection<string> usedNames)
        {
            switch (Role)
            {
                case BuildingRole.Resource:
                    Name = DetermineUniqueName(usedNames, "Resource Building");
                    break;
                case BuildingRole.Factory:
                    Name = DetermineUniqueName(usedNames, "Factory");
                    break;
                case BuildingRole.Research:
                    Name = DetermineUniqueName(usedNames, "Tech Building");
                    break;
                case BuildingRole.Defense:
                    Name = DetermineUniqueName(usedNames, "Defense Building");
                    break;
                case BuildingRole.Utility:
                    Name = DetermineUniqueName(usedNames, "Utility Building");
                    break;
            }
        }

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
