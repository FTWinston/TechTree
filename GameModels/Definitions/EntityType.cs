using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public abstract class Purchasable
    {
        public string Name { get; internal set; }
        public string Symbol { get; internal set; }

        public int BuildTime { get; internal set; }

        public int MineralCost { get; internal set; }
        public int VespineCost { get; internal set; }
        public int SupplyCost { get; internal set; }

        private BuildingType prerequisite;
        public BuildingType Prerequisite
        {
            get { return prerequisite; }
            internal set
            {
                if (prerequisite != null)
                    prerequisite.Unlocks.Remove(this);

                prerequisite = value;
                if (prerequisite != null)
                    prerequisite.Unlocks.Add(this);
            }
        }
    }

    public abstract class EntityType : Purchasable
    {
        public int Health { get; internal set; }
        public int Armor { get; internal set; }
        public int Mana { get; internal set; }

        public int VisionRange { get; internal set; }
        public bool IsDetector { get; internal set; }

        // TODO: decide if we really do want to use action points for movement etc
        public int ActionPoints { get; internal set; }

        private List<Feature> features;
        public IReadOnlyList<Feature> Features { get { return features; } }

        protected EntityType()
        {
            features = new List<Feature>();
        }

        internal void AddFeature(Feature feature)
        {
            features.Add(feature);
            feature.EntityDefinition = this;
        }

        internal void RemoveAllFeatures()
        {
            features.Clear();
        }

        internal bool Populated { get; set; }
    }

    public abstract class EntityType<I> : EntityType
        where I : Entity
    {
        public I CreateInstance()
        {
            throw new NotImplementedException();
        }
    }
}
