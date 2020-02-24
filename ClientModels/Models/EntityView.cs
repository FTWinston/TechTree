using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Instances;

namespace ObjectiveStrategy.ClientModels.Models
{
    public abstract class EntityView
    {
        public abstract string Type { get; }

        public abstract uint ID { get; }

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
        public EntityView(Player viewPlayer, TEntity entity)
        {
            ViewPlayer = viewPlayer;
            Entity = entity;
        }

        protected Player ViewPlayer { get; }

        protected TEntity Entity { get; }

        public override uint ID => Entity.ID;

        public override int Owner => Entity.Owner.ID;

        public override int Health => Entity.Health;

        public override int MaxHealth => Entity.BaseDefinition.Health;

        public override int Mana => Entity.Owner == ViewPlayer ? Entity.Mana : 0;

        public override int MaxMana => Entity.Owner == ViewPlayer ? Entity.BaseDefinition.Mana : 0;

        public override int Armor => Entity.BaseDefinition.Armor;
    }
}
