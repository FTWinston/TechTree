using GameModels.Definitions;
using GameModels.Instances;
using System.Collections.Generic;
using System.Linq;

namespace GameModels
{
    public class Player
    {
        public int ID { get; }
        public TechTree TechTree { get; }

        public List<Building> Buildings { get; }
        public List<Unit> Units { get; }

        public List<BuildingType> AvailableBuildings { get; }
        public List<UnitType> AvailableUnits { get; }
        public List<Research> CompletedResearch { get; }

        public Player(int id, TechTree tree)
        {
            ID = id;
            TechTree = tree;
            Buildings = new List<Building>();
            Units = new List<Unit>();

            AvailableBuildings = new List<BuildingType>();
            AvailableUnits = new List<UnitType>();
            CompletedResearch = new List<Research>();
        }

        public void BuildingCompleted(Building b)
        {
            foreach (var typeID in b.Definition.Unlocks)
            {
                if (TechTree.Buildings.TryGetValue(typeID, out var buildingType))
                {
                    if (!AvailableBuildings.Contains(buildingType))
                        AvailableBuildings.Add(buildingType);
                }

                else if (TechTree.Units.TryGetValue(typeID, out var unitType))
                {
                    if (!AvailableUnits.Contains(unitType))
                        AvailableUnits.Add(unitType);
                }
            }
        }

        public void BuildingDestroyed(Building b)
        {
            if (!b.Definition.Prerequisite.HasValue)
                return;

            var prerequisite = TechTree.Buildings[b.Definition.Prerequisite.Value];

            if (!Buildings.Any(other => other.Definition == prerequisite))
                AvailableBuildings.Remove(b.Definition);
        }

        public void ResearchComplete(Research r)
        {
            CompletedResearch.Add(r);

            // for all features that this research unlocks, enable them on all units that have that feature
            foreach (Feature unlock in r.Unlocks)
            {
                EntityType relatedEntityType = unlock.EntityDefinition;

                if (relatedEntityType is UnitType)
                    foreach (Unit u in Units.Where(u => u.Definition == relatedEntityType))
                        u.LockedFeatures.Remove(unlock);
                else if (relatedEntityType is BuildingType)
                    foreach (Building b in Buildings.Where(b => b.Definition == relatedEntityType))
                        b.LockedFeatures.Remove(unlock);
            }
        }
    }
}
