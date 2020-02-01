using System.Collections.Generic;

namespace GameModels.Definitions
{
    public interface IEntityType : IPurchasable
    {
        uint ID { get; }

        int Health { get; }

        int Armor { get; }

        int Mana { get; }


        int VisionRange { get; }

        bool IsDetector { get; }


        List<uint> UpgradesTo { get; }

        uint? UpgradesFrom { get; }


        List<Feature> Features { get; }
    }
}
