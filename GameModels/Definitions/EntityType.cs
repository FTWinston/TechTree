using System.Collections.Generic;

namespace GameModels.Definitions
{
    public abstract class EntityType : Purchasable, IEntityType
    {
        protected EntityType(IEntityType copyFrom)
            : base(copyFrom)
        {
            Health = copyFrom.Health;
            Armor = copyFrom.Armor;
            Mana = copyFrom.Mana;

            VisionRange = copyFrom.VisionRange;
            IsDetector = copyFrom.IsDetector;

            Features = new List<Feature>(copyFrom.Features);
        }

        public int Health { get; internal set; }

        public int Armor { get; internal set; }

        public int Mana { get; internal set; }


        public int VisionRange { get; internal set; }

        public bool IsDetector { get; internal set; }


        public List<Feature> Features { get; }
    }
}
