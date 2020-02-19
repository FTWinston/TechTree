using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public abstract class EntityView
    {

    }

    public abstract class EntityView<TEntity> : EntityView
        where TEntity : Entity
    {
        public EntityView(TEntity entity)
        {
            Entity = entity;
        }

        protected TEntity Entity { get; }

        public int Owner => Entity.Owner.ID;

        public int Health => Entity.Health;

        public int MaxHealth => Entity.BaseDefinition.Health;

        public int Mana => Entity.Mana;

        public int MaxMana => Entity.BaseDefinition.Mana;

        public int Armor => Entity.BaseDefinition.Armor;
    }
}
