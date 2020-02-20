using ObjectiveStrategy.GameModels.Definitions;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class TreeEntityView<TEntityType> : PurchasableView<TEntityType>
        where TEntityType : EntityType
    {
        public TreeEntityView(TEntityType entityType)
            : base(entityType)
        { }

        public int Health => Item.Health;

        public int Mana => Item.Mana;

        public int Armor => Item.Armor;
    }
}
