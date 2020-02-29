using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameGeneration.TreeGeneration
{
    public abstract class EntityBuilder : IEntityType
    {
        public EntityBuilder(Random random, uint id, string symbol)
        {
            Random = random;
            ID = id;
            Symbol = symbol;
        }

        protected Random Random { get; }

        public string Name { get; set; } = string.Empty;

        public string Symbol { get; }

        public uint ID { get; set; }

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

        public List<Feature> LockedFeatures { get; } = new List<Feature>();

        public abstract void AllocateName(ICollection<string> usedNames);

        protected static string DetermineUniqueName(ICollection<string> usedNames, string baseName)
        {
            var number = 1;
            string name;
            do
            {
                name = $"{baseName} #{number++}";
            } while (usedNames.Contains(name));

            usedNames.Add(name);
            return name;
        }
    }
}
