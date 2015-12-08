using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public class BuildingType : EntityType<Building>
    {
        public BuildingType Prerequisite { get; private set; }
        public List<BuildingType> Unlocks { get; private set; }

        // TODO: decide if building units, researching and upgrading could become part of building features
        public List<BuildingType> UpgradesTo { get; private set; }
        public BuildingType UpgradesFrom { get; private set; }

        public List<UnitType> Builds { get; private set; }
        public List<Research> Researches { get; private set; }

        public BuildingType()
        {
            Unlocks = new List<BuildingType>();
            UpgradesTo = new List<BuildingType>();
            Builds = new List<UnitType>();
            Researches = new List<Research>();

            ActionPoints = 0;
        }
    }
}
