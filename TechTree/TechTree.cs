using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTree
{
    public class TechTree
    {
        const int absMinTreeBreadth = 1, absMaxTreeBreadth = 100, minTreeBreadth = 1, maxTreeBreadth = 60;
        const int absMinBuildings = 3, absMaxBuildings = 50, minBuildings = 8, maxBuildings = 20;

        public TreeNode RootNode;
        Random r;

        public TechTree(int seed, int? treeBreadth = null, int? numBuildings = null)
        {
            r = new Random(seed);

            if (!treeBreadth.HasValue || treeBreadth < absMinTreeBreadth || treeBreadth > absMaxTreeBreadth)
                treeBreadth = r.Next(minTreeBreadth - 1, maxTreeBreadth) + 1;

            if (!numBuildings.HasValue || numBuildings < absMinBuildings || numBuildings > absMaxBuildings)
                numBuildings = r.Next(minBuildings - 1, maxBuildings) + 1;

            // While SC refinerys don't require a command center, I don't think replicating that matters too much at this point.
            // So a single "command center" style building will be fine
            RootNode = new TreeNode();

            for (int i = 2; i <= numBuildings; i++)
            {
                TreeNode parent = SelectNode(treeBreadth.Value);
                TreeNode newNode = new TreeNode();
                parent.Unlocks.Add(newNode);
                newNode.Prerequisites.Add(parent);
            }
        }

        private TreeNode SelectNode(int treeBreadth)
        {
            TreeNode current = RootNode;
            while (true)
                if (current.Unlocks.Count > 0 && r.Next(absMaxTreeBreadth) >= treeBreadth)
                    current = current.Unlocks[r.Next(current.Unlocks.Count)];
                else
                    break;
            
            return current;
        }
    }

    public class TreeNode
    {
        public List<TreeNode> Prerequisites = new List<TreeNode>(), Unlocks = new List<TreeNode>();
    }
}
