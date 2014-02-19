using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class TechTree
    {
        public List<BuildingInfo> AllBuildings = new List<BuildingInfo>();
        public List<UnitInfo> AllUnits = new List<UnitInfo>();
        public List<ResearchInfo> AllResearch = new List<ResearchInfo>();

        public List<BuildingInfo> DefaultBuildings = new List<BuildingInfo>();

        public int MaxTreeColumn { get; internal set; }
        public int MaxTreeRow { get; internal set; }

        public static TechTree Generate(int seed)
        {
            Random r = new Random(seed);

            TechTree tree = new TechTree();
            TreeGenerator treeGen = new TreeGenerator(tree, r);
            treeGen.SortLayout();

            tree.AllBuildings = treeGen.AllNodes;
            tree.DefaultBuildings = treeGen.RootNodes;

            // that's the building tree sorted. Now we need to determine what units & research they produce

            return tree;
        }
    }
}
