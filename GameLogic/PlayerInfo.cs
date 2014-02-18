
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class PlayerInfo
    {
        public TechTree Tree { get; private set; }
        public int Minerals { get; set; }
        public int Vespine { get; set; }

        public PlayerInfo(TechTree tree)
        {
            Tree = tree;
            
            foreach (var building in tree.DefaultBuildings)
                Unlock(building);
        }

        List<BuyableInfo> UnlockedItems = new List<BuyableInfo>();

        public bool IsUnlocked(BuyableInfo item)
        {
            return UnlockedItems.BinarySearch(item) >= 0;
        }

        public void Unlock(BuyableInfo item)
        {
            int pos = UnlockedItems.BinarySearch(item);
            if (pos < 0)
                UnlockedItems.Insert(~pos, item);
        }
    }
}
