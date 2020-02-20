using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class SelectableView<TSelectable>
        where TSelectable : ISelectable
    {
        public SelectableView(TSelectable item)
        {
            Item = item;
        }

        protected TSelectable Item { get; }

        public string Name => Item.Name;

        public string Symbol => Item.Symbol;
    }
}
