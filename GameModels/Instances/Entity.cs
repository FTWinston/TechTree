using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Instances
{
    public abstract class Entity
    {
        protected Entity(uint id, Player owner, Cell location)
        {
            ID = id;
            Owner = owner;
            this.location = location;
        }

        public uint ID { get; }

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

        public int Health { get; set; }

        public int Mana { get; set; }

        public abstract EntityType BaseDefinition { get; }

        public List<Feature> LockedFeatures { get; } = new List<Feature>();

        public List<Tuple<IStatusEffect, int>> StatusEffects { get; } = new List<Tuple<IStatusEffect, int>>();

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

        public override EntityType BaseDefinition => Definition;

        public Entity(uint id, Player owner, T definition, Cell location)
            : base(id, owner, location)
        {
            Definition = definition;

            foreach (Feature f in Definition.Features)
                if (f.UnlockedBy != null && !owner.CompletedResearch.Contains(f.UnlockedBy))
                    LockedFeatures.Add(f);
        }
    }
}
