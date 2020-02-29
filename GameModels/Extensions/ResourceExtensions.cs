using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Extensions
{
    public static class ResourceExtensions
    {
        public static bool HasValue(this Dictionary<ResourceType, int> resources, Dictionary<ResourceType, int> cost)
        {
            foreach (var kvp in cost)
                if (!resources.TryGetValue(kvp.Key, out var available) || available < kvp.Value)
                    return false;

            return true;
        }

        public static void AddValue(this Dictionary<ResourceType, int> resources, ResourceType type, int value)
        {
            if (!resources.TryGetValue(type, out var current))
                current = 0;

            resources[type] = current + value;
        }

        public static bool SubtractValue(this Dictionary<ResourceType, int> resources, ResourceType type, int value)
        {
            if (!resources.TryGetValue(type, out var current))
                current = 0;

            var updated = current - value;

            bool success;

            if (updated < 0)
            {
                success = false;
                updated = 0;
            }
            else
            {
                success = true;
            }

            resources[type] = updated;
            return success;
        }


        public static void AddValue(this Dictionary<ResourceType, int> resources, Dictionary<ResourceType, int> toAdd)
        {
            foreach (var kvp in toAdd)
                AddValue(resources, kvp.Key, kvp.Value);
        }

        public static bool SubtractValue(this Dictionary<ResourceType, int> resources, Dictionary<ResourceType, int> toRemove)
        {
            bool success = true;

            foreach (var kvp in toRemove)
                success |= SubtractValue(resources, kvp.Key, kvp.Value);

            return success;
        }
    }
}
