using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels
{
    public class Player
    {
        public int ID { get; }

        public TechTree TechTree { get; }

        public Dictionary<ResourceType, int> Resources { get; } = new Dictionary<ResourceType, int>();

        public HashSet<Building> Buildings { get; } = new HashSet<Building>();
        
        public HashSet<Unit> Units { get; } = new HashSet<Unit>();

        public HashSet<BuildingType> AvailableBuildings { get; } = new HashSet<BuildingType>();
        
        public HashSet<UnitType> AvailableUnits { get; } = new HashSet<UnitType>();
        
        public HashSet<Research> CompletedResearch { get; } = new HashSet<Research>();

        public List<int> ObjectiveIndices { get; } = new List<int>();

        public Dictionary<Cell, BuildingSnapshot?> SeenCells { get; } = new Dictionary<Cell, BuildingSnapshot?>();

        public Player(int id, TechTree tree)
        {
            ID = id;
            TechTree = tree;
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

            /*
            // for all features that this research unlocks, enable them on all units that have that feature
            foreach (Feature unlock in r.Unlocks)
            {
                foreach (var unitType in TechTree.Units.Values)
                    if (unitType.LockedFeatures.Contains(unlock))
                    {
                        unitType.LockedFeatures.Remove(unlock);
                        unitType.Features.Add(unlock);
                        unlock.Initialize(unitType);

                        foreach (var unit in Units.Where(u => u.Definition == unitType))
                            unlock.Unlock(unit);
                   }

                foreach (var buildingType in TechTree.Buildings.Values)
                    if (buildingType.LockedFeatures.Contains(unlock))
                    {
                        buildingType.LockedFeatures.Remove(unlock);
                        buildingType.Features.Add(unlock);
                        unlock.Initialize(buildingType);

                        foreach (var building in Buildings.Where(b => b.Definition == buildingType))
                            unlock.Unlock(building);
                    }
            }
            */
        }
    }
}
