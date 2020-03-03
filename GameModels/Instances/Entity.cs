using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Instances
{
    public abstract class Entity
    {
        protected Entity(uint id, Player owner, Cell location)
        {
            ID = id;
            Owner = owner;
            Location = location;
        }

        public uint ID { get; }

        public Player Owner { get; set; }

        public Cell Location { get; set; }

        public int Health { get; set; }

        public int Mana { get; set; }

        public abstract EntityType BaseDefinition { get; }

        [JsonIgnore]
        public HashSet<Cell> VisibleCells { get; set; } = new HashSet<Cell>();

        public Dictionary<Feature, Dictionary<string, int>> FeatureData { get; } = new Dictionary<Feature, Dictionary<string, int>>();

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
        }
    }
}
