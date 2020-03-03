using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public abstract class EntityType : Purchasable, IEntityType
    {
        protected EntityType(IEntityType copyFrom)
            : base(copyFrom)
        {
            ID = copyFrom.ID;
            Health = copyFrom.Health;
            Armor = copyFrom.Armor;
            Mana = copyFrom.Mana;

            VisionRange = copyFrom.VisionRange;
            IsDetector = copyFrom.IsDetector;

            UpgradesTo = new List<uint>(copyFrom.UpgradesTo);
            UpgradesFrom = copyFrom.UpgradesFrom;

            Features = new List<Feature>(copyFrom.Features);

            LockedFeatures = new List<Feature>(copyFrom.LockedFeatures);
        }

        public uint ID { get; internal set; }

        public int Health { get; internal set; }

        public int Armor { get; internal set; }

        public int Mana { get; internal set; }


        public int VisionRange { get; internal set; }

        public bool IsDetector { get; internal set; }


        public List<uint> UpgradesTo { get; }

        public uint? UpgradesFrom { get; }

        [JsonIgnore]
        public List<Feature> Features { get; }

        [JsonIgnore]
        public List<Feature> LockedFeatures { get; }

        [JsonPropertyName("Features")]
        public List<FeatureDTO> FeatureDTOs
        {
            get
            {
                return Features
                    .Select(f => f.ToDTO())
                    .ToList();
            }
            set
            {
                Features.Clear();
                Features.AddRange
                (
                    value
                        .Select(f => f.ToFeature())
                        .ToList()
                );
            }
        }

        [JsonPropertyName("LockedFeatures")]
        public List<FeatureDTO> LockedFeatureDTOs
        {
            get
            {
                return LockedFeatures
                    .Select(f => f.ToDTO())
                    .ToList();
            }
            set
            {
                LockedFeatures.Clear();
                LockedFeatures.AddRange
                (
                    value
                        .Select(f => f.ToFeature())
                        .ToList()
                );
            }
        }
    }
}
