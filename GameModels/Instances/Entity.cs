using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Instances
{
    public abstract class Entity
    {
        public Player Owner { get; set; }
        public List<Feature> LockedFeatures { get; protected set; }
    }

    public abstract class Entity<T> : Entity where T: EntityType
    {
        public T Definition { get; private set; }

        public Entity(Player p, T definition)
        {
            Definition = definition;
            Owner = p;

            LockedFeatures = new List<Feature>();
            foreach (Feature f in Definition.Features)
                if (f.UnlockedBy != null && !p.CompletedResearch.Contains(f.UnlockedBy))
                    LockedFeatures.Add(f);
        }
    }
}
