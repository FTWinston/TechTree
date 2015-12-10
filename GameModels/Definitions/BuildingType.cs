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
        public List<EntityType> Unlocks { get; private set; }

        // TODO: decide if building units, researching and upgrading could become part of building features
        public List<BuildingType> UpgradesTo { get; private set; }

        private BuildingType upgradesFrom;
        public BuildingType UpgradesFrom
        {
            get { return upgradesFrom; }
            internal set
            {
                if (upgradesFrom != null)
                    upgradesFrom.UpgradesTo.Remove(this);

                upgradesFrom = value;
                upgradesFrom.UpgradesTo.Add(this);
            }
        }

        public List<UnitType> Builds { get; private set; }
        public List<Research> Researches { get; private set; }

        public int DisplayRow { get; internal set; }
        public int DisplayColumn { get; internal set; }

        public BuildingType()
        {
            Unlocks = new List<EntityType>();
            UpgradesTo = new List<BuildingType>();
            Builds = new List<UnitType>();
            Researches = new List<Research>();

            ActionPoints = 0;
        }
    }
}
