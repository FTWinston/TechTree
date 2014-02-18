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

            double factoryChance = 0.35, techChance = factoryChance + 0.4, utilityChance = techChance + 0.25;
            foreach (var bi in tree.AllBuildings)
            {
                bi.TreeColor = System.Drawing.Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                if (bi.Prerequisites.Count == 0)
                    tree.DefaultBuildings.Add(bi);

                double d = r.NextDouble();
                if (d < factoryChance)
                    bi.Type = BuildingInfo.BuildingType.Factory;
                else if (d < techChance)
                    bi.Type = BuildingInfo.BuildingType.Tech;
                else
                    bi.Type = BuildingInfo.BuildingType.Utility;
            }
            
            // one of the default buildings must be the "command center" equivalent
            tree.DefaultBuildings[r.Next(tree.DefaultBuildings.Count)].Type = BuildingInfo.BuildingType.Factory;


            // that's the building tree sorted. Now we need to determine what units & research they produce

            return tree;
        }
    }
}
