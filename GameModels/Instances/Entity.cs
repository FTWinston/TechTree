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
        public List<Tuple<IStatusEffect, int>> StatusEffects { get; protected set; }

        public void AddEffect(IStatusEffect effect)
        {
            StatusEffects.Add(new Tuple<IStatusEffect, int>(effect, effect.Duration));
            effect.BeforeFirstTick(this);
        }

        public void RemoveEffect(IStatusEffect effect)
        {
            StatusEffects.RemoveAll(e => e.Item1 == effect);
        }

        public void RemoveAllEffects()
        {
            StatusEffects.Clear();
        }
    }

    public abstract class Entity<T> : Entity where T: EntityType
    {
        public T Definition { get; private set; }

        public Entity(Player owner, T definition)
        {
            Definition = definition;
            Owner = owner;

            LockedFeatures = new List<Feature>();
            foreach (Feature f in Definition.Features)
                if (f.UnlockedBy != null && !owner.CompletedResearch.Contains(f.UnlockedBy))
                    LockedFeatures.Add(f);

            StatusEffects = new List<Tuple<IStatusEffect, int>>();
        }
    }
}
