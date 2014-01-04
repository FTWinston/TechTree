using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TechTree
{
    public class TechTree
    {
        const int absMinTreeBreadth = 1, absMaxTreeBreadth = 100, minTreeBreadth = 1, maxTreeBreadth = 60;
        const int absMinBuildings = 3, absMaxBuildings = 50, minBuildings = 8, maxBuildings = 20;

        public TreeNode RootNode { get; private set; }
        public List<TreeNode> AllNodes { get; private set; }
        public int MaxDepth { get; private set; }
        public int MaxRowPos { get; private set; }

        Random r;

        public TechTree(int seed) : this(seed, null, null) { }
        public TechTree(int seed, int? treeBreadth, int? numBuildings)
        {
            r = new Random(seed);

            if (!treeBreadth.HasValue || treeBreadth < absMinTreeBreadth || treeBreadth > absMaxTreeBreadth)
                treeBreadth = r.Next(minTreeBreadth - 1, maxTreeBreadth) + 1;

            if (!numBuildings.HasValue || numBuildings < absMinBuildings || numBuildings > absMaxBuildings)
                numBuildings = r.Next(minBuildings - 1, maxBuildings) + 1;

            // While SC refinerys don't require a command center, I don't think replicating that matters too much at this point.
            // So a single "command center" style building will be fine
            RootNode = new TreeNode(this);
            MaxDepth = MaxRowPos = 0;

            for (int i = 2; i <= numBuildings; i++)
            {
                TreeNode parent = SelectNode(treeBreadth.Value);
                TreeNode newNode = new TreeNode(this);
                parent.Unlocks.Add(newNode);
                newNode.Prerequisites.Add(parent);
            }

            SortByDepth();
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

        private void SortByDepth()
        {
            var nodesByDepth = new SortedList<int, int>();
            AllNodes = new List<TreeNode>();
            var toProcess = new List<TreeNode>();
            toProcess.Add(RootNode);

            while (toProcess.Count > 0)
            {
                TreeNode node = toProcess[0]; toProcess.RemoveAt(0);

                int depth = Depth(node);
                int numAtDepth;
                if (!nodesByDepth.TryGetValue(depth, out numAtDepth))
                    numAtDepth = 0;

                node.Depth = depth;
                node.RowPos = numAtDepth;

                nodesByDepth[depth] = ++numAtDepth;
                AllNodes.Add(node);

                foreach (TreeNode child in node.Unlocks)
                    if (!toProcess.Contains(child))
                        toProcess.Add(child);
            }
        }

        private static int Depth(TreeNode node)
        {
            if (node.Prerequisites.Count == 0)
                return 0;

            int depth = 0;
            foreach (TreeNode parent in node.Prerequisites)
                depth = Math.Max(depth, Depth(parent));
            return depth + 1;
        }

        public void SortLayout()
        {
            SortByDescendents(RootNode);
        }

        private int SortByDescendents(TreeNode node)
        {
            node.Unlocks.Sort((n1, n2) => -n1.CountDescendents().CompareTo(n2.CountDescendents()));

            int maxRowPos = node.RowPos;
            foreach (var child in node.Unlocks)
            {
                child.RowPos = maxRowPos;
                maxRowPos = SortByDescendents(child) + 1;
            }

            if (node.Unlocks.Count > 0)
            {
                maxRowPos--;
                node.RowPos = (node.RowPos + maxRowPos) / 2;
            }
            return maxRowPos;
        }

        private static TreeNode FindNodeAtPos(List<TreeNode> nodes, int testPos)
        {
            foreach (TreeNode test in nodes)
                if (test.RowPos == testPos)
                    return test;
            return null;
        }

        public class TreeNode : IComparable<TreeNode>
        {
            public TechTree Tree { get; private set; }
            public Color Color { get; private set; }
            public TreeNode(TechTree tree) { Tree = tree; Color = Color.FromArgb(Tree.r.Next(256), Tree.r.Next(256), Tree.r.Next(256)); }

            public List<TreeNode> Prerequisites = new List<TreeNode>(), Unlocks = new List<TreeNode>();

            private int depth, rowPos;
            public int Depth
            {
                get { return depth; }
                set { depth = value; Tree.MaxDepth = Math.Max(Tree.MaxDepth, depth); }
            }
            public int RowPos
            {
                get { return rowPos; }
                set { rowPos = value; Tree.MaxRowPos = Math.Max(Tree.MaxRowPos, rowPos); }
            }

            public int CompareTo(TreeNode other)
            {
                var result = Depth.CompareTo(other.Depth);
                if (result != 0)
                    return result;

                return RowPos.CompareTo(other.RowPos);
            }

            internal int CountDescendents()
            {
                int num = Unlocks.Count;
                foreach (var node in Unlocks)
                    num += node.CountDescendents();
                return num;
            }
        }
    }
}
