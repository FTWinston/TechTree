using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class TreeItemView<TItem>
        where TItem : Purchasable
    {
        public TreeItemView(TItem item)
        {
            Item = item;
        }

        protected TItem Item { get; }

        public string Name => Item.Name;

        public string Symbol => Item.Symbol;

        public int BuildTime => Item.BuildTime;

        public Dictionary<ResourceType, int> Cost => Item.Cost;

    }
}
