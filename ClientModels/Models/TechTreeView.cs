using ObjectiveStrategy.GameModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.ClientModels.Models
{
    public class TechTreeView
    {
        public TechTreeView(TechTree techTree)
        {
            Buildings = techTree.Buildings
                .ToDictionary(kvp => kvp.Key, kvp => new TreeBuildingView(techTree, kvp.Value));
        }

        public Dictionary<uint, TreeBuildingView> Buildings { get; }
    }
}
