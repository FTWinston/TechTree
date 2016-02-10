using GameModels.Instances;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public class BuildingType : EntityType<Building>
    {
        [JsonIgnore]
        public List<Purchasable> Unlocks { get; private set; }

        // TODO: decide if building units, researching and upgrading could become part of building features
        [JsonIgnore]
        public List<BuildingType> UpgradesTo { get; private set; }

        private BuildingType upgradesFrom;
        [JsonIgnore]
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

        [JsonProperty("UpgradesFrom",  NullValueHandling = NullValueHandling.Ignore)]
        public int? UpgradesFromNumber { get { if (upgradesFrom == null) return null; return upgradesFrom.BuildingNumber; } }

        [JsonIgnore]
        public List<UnitType> Builds { get; private set; }
        [JsonIgnore]
        public List<Research> Researches { get; private set; }

        public int DisplayRow { get; internal set; }
        public int DisplayColumn { get; internal set; }

        internal int BuildingNumber { get; set; }

        public BuildingType()
        {
            Unlocks = new List<Purchasable>();
            UpgradesTo = new List<BuildingType>();
            Builds = new List<UnitType>();
            Researches = new List<Research>();

            ActionPoints = 0;
        }
    }
}
