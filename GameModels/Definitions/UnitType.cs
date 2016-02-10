using GameModels.Definitions.Features;
using GameModels.Instances;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public class UnitType : EntityType<Unit>
    {
        private BuildingType builtBy;
        [JsonIgnore]
        public BuildingType BuiltBy
        {
            get { return builtBy; }
            internal set
            {
                if (builtBy != null)
                    builtBy.Builds.Remove(this);

                builtBy = value;
                builtBy.Builds.Add(this);
                builtBy.AddFeature(new Build(this));
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BuiltByID { get { return builtBy == null ? null : builtBy.Symbol; } }

        [Flags]
        public enum UnitFlags
        {
            None = 0,
            Agile = 1, // ignores difficult terrain
            Flying = 2, // ignores difficult and blocking terrain
            AttacksAir = 4,
            AttacksGround = 8,
        }

        [JsonIgnore]
        public UnitFlags Flags { get; internal set; }
    }
}
