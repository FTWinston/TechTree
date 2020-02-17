using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Instances
{
    public abstract class Entity
    {
        public Player Owner { get; set; }

        private Cell location;
        public Cell Location
        {
            get => location;
            set
            {
                if (value.Entity != null && value.Entity != this)
                {
                    throw new InvalidOperationException("Cannot move entity to non-empty cell");
                }

                if (location != null && location.Entity == this)
                {
                    location.Entity = null;
                }

                location = value;
                location.Entity = this;
            }
        }

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
