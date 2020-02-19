using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class TreeEntityView<TEntityType> : TreeItemView<TEntityType>
        where TEntityType : EntityType
    {
        public TreeEntityView(TEntityType entityType)
            : base(entityType)
        { }

        public int Health => Item.Health;
    }
}
