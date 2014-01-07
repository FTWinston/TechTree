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

            PositionNodes();
        }

        private static int Sort(TreeNode n1, TreeNode n2)
        {
            int depth = n1.Depth.CompareTo(n2.Depth);
            if (depth != 0)
                return depth;
            return n1.RowPos.CompareTo(n2.RowPos);
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

        private void PositionNodes()
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
            AllNodes.Sort((n1, n2) => Sort(n1, n2));
            //CondenseLayout(RootNode);
        }

        private int SortByDescendents(TreeNode node)
        {
            node.Unlocks.Sort((n1, n2) => -n1.CountDescendents().CompareTo(n2.CountDescendents()));

            int maxRowPos = node.RowPos;
            if (node.Unlocks.Count == 0)
                return maxRowPos;

            TreeNode lastChild = null;
            foreach (var child in node.Unlocks)
            {
                child.RowPos = maxRowPos;
                maxRowPos = SortByDescendents(child) + 1;
                lastChild = child;
            }

            maxRowPos--;
            node.RowPos = (node.RowPos + maxRowPos) / 2;
            return maxRowPos;
        }

        public void CondenseLayout()
        {
            while (CondenseLayout(RootNode))
                ;
        }

        private bool CondenseLayout(TreeNode node)
        {
            // if any of this node's descendents can have its entire sub-tree's rowpos reduced by 1, then do that.
            // that will make more efficient use of space.

            bool retVal = false;
            foreach (var child in node.Unlocks)
            {
                if (CanDecrementRowPos(node, child) && child.RowPos > node.RowPos) // use child.MaxRowPos instead of child.RowPos here?
                {
                    DecrementRowPos(child);
                    retVal = true;
                }

                if ( CondenseLayout(child) )
                    retVal = true;
            }
            return retVal;
        }

        private bool CanDecrementRowPos(TreeNode parent, TreeNode node)
        {
            if (node.RowPos == 0)
                return false;
            int pos = AllNodes.IndexOf(node);
            TreeNode prev = AllNodes[pos > 0 ? pos - 1 : 0];

            if (prev.Depth != node.Depth || prev.RowPos >= node.RowPos - 1)
                return false;

            foreach (var child in node.Unlocks)
                if ( !CanDecrementRowPos(node, child) )
                    return false;

            return true;
        }

        private void DecrementRowPos(TreeNode node)
        {
            node.RowPos--;
            foreach (var child in node.Unlocks)
                DecrementRowPos(child);
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
