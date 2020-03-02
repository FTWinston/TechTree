using ObjectiveStrategy.GameModels.Definitions;
using System.Collections.Generic;

namespace ObjectiveStrategy.ClientModels.Models
{
    public abstract class PurchasableView<TPurchasable> : SelectableView<TPurchasable>
        where TPurchasable : Purchasable
    {
        public PurchasableView(TPurchasable item)
            : base(item)
        {
            
        }

        public int BuildTime => Item.BuildTime;

        public uint? Prerequisite => Item.Prerequisite;

        public Dictionary<ResourceType, int> Cost => Item.Cost;
    }
}
