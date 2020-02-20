using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public abstract class EntityView
    {
        public abstract string Type { get; }

        public abstract int Owner { get; }

        public abstract int Health { get; }

        public abstract int MaxHealth { get; }

        public abstract int Mana { get; }

        public abstract int MaxMana { get; }

        public abstract int Armor { get; }
    }

    public abstract class EntityView<TEntity> : EntityView
        where TEntity : Entity
    {
        public EntityView(TEntity entity)
        {
            Entity = entity;
        }

        protected TEntity Entity { get; }

        public override int Owner => Entity.Owner.ID;

        public override int Health => Entity.Health;

        public override int MaxHealth => Entity.BaseDefinition.Health;

        public override int Mana => Entity.Mana;

        public override int MaxMana => Entity.BaseDefinition.Mana;

        public override int Armor => Entity.BaseDefinition.Armor;
    }
}
