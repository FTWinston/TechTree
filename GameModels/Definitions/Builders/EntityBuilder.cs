using System;
using System.Collections.Generic;

namespace GameModels.Definitions.Builders
{
    public abstract class EntityBuilder : IEntityType
    {
        public EntityBuilder(Random random, string symbol)
        {
            Random = random;
            Symbol = symbol;
        }

        protected Random Random { get; }

        public string Name { get; set; }

        public string Symbol { get; }

        public int Tier { get; set; } = 1;


        public int Health { get; set; }

        public int Armor { get; set; }

        public int Mana { get; set; }


        public int BuildTime { get; set; } = 1;

        public Dictionary<ResourceType, int> Cost { get; set; } = new Dictionary<ResourceType, int>();

        public uint? Prerequisite { get; set; }


        public List<uint> UpgradesTo { get; } = new List<uint>();

        public uint? UpgradesFrom { get; set; }


        public int VisionRange { get; set; } = 4;

        public bool IsDetector { get; set; }

        public List<Feature> Features { get; } = new List<Feature>();
    }
}
