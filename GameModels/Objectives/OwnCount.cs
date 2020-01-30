using System.Linq;

namespace GameModels.Objectives
{
    public class OwnCount : IObjective
    {
        public OwnCount(uint entityTypeID, int minNumber, int value)
        {
            EntityTypeID = entityTypeID;
            MinNumber = minNumber;
            Value = value;
        }

        public uint EntityTypeID { get; }

        public int MinNumber { get; }

        public int Value { get; }

        public bool IsSatisfied(Player player)
        {
            if (player.TechTree.Buildings.TryGetValue(EntityTypeID, out var buildingType))
            {
                return player.Buildings
                    .Where(b => b.Definition == buildingType)
                    .Count() >= MinNumber;
            }

            if (player.TechTree.Units.TryGetValue(EntityTypeID, out var unitType))
            {
                return player.Units
                    .Where(u => u.Definition == unitType)
                    .Count() >= MinNumber;
            }

            return false;
        }
    }
}
