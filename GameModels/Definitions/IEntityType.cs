using System.Collections.Generic;

namespace GameModels.Definitions
{
    public interface IEntityType : IPurchasable
    {
        int Health { get; }

        int Armor { get; }

        int Mana { get; }


        int VisionRange { get; }

        bool IsDetector { get; }


        List<Feature> Features { get; }
    }
}
