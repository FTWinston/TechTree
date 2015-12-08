using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels
{
    public class Player
    {
        public TechTree TechTree { get; private set; }

        public List<Building> Buildings { get; private set; }
        public List<Unit> Units { get; private set; }

        public List<BuildingType> AvailableBuildings { get; private set; }
        public List<Research> CompletedResearch { get; private set; }

        public Player(TechTree tree)
        {
            TechTree = tree;
            Buildings = new List<Building>();
            Units = new List<Unit>();

            AvailableBuildings = new List<BuildingType>();
            CompletedResearch = new List<Research>();
        }

        public void BuildingCompleted(Building b)
        {
            foreach (var type in b.Definition.Unlocks)
                if (!AvailableBuildings.Contains(type))
                    AvailableBuildings.Add(type);
        }

        public void BuildingDestroyed(Building b)
        {
            if (b.Definition.Prerequisite == null)
                return;

            if (Buildings.Find(x => x.Definition == b.Definition.Prerequisite) == null)
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
