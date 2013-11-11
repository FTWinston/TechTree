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

        public TechTree(int seed, int? treeBreadth = null, int? numBuildings = null)
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
            var nodesByDepth = new SortedList<int, List<TreeNode>>();

            foreach (var node in AllNodes)
            {
                List<TreeNode> nodesAtDepth;
                if (!nodesByDepth.TryGetValue(node.Depth, out nodesAtDepth))
                {
                    nodesAtDepth = new List<TreeNode>();
                    nodesByDepth.Add(node.Depth, nodesAtDepth);
                }

                nodesAtDepth.Add(node);
            }

            bool anyChange = false;
            for (int iteration = 0; iteration < 1; iteration++)
            {
                // operate on each depth in turn
                foreach (var depth in nodesByDepth)
                {
                    // determine the "forces" acting on each node
                    var nodes = depth.Value;
                    int largestIndex = -1; float largestForce = 0;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        float parentForce = 0, childForce = 0;
                        foreach (var parent in node.Prerequisites)
                            parentForce += parent.RowPos - node.RowPos;

                        foreach (var child in node.Unlocks)
                            childForce += child.RowPos - node.RowPos;

                        if (node.Prerequisites.Count > 0)
                            parentForce /= node.Prerequisites.Count;
                        if (node.Unlocks.Count > 0)
                            childForce /= node.Unlocks.Count;

                        var force = parentForce + childForce;

                        if (Math.Abs(force) > Math.Abs(largestForce))
                        {
                            largestForce = force;
                            largestIndex = i;
                        }
                    }

                    if (Math.Abs(largestForce) <= 0.5f)
                        continue;

                    // apply the largest force only to the nodes at this depth .. and only move one step, not the full distance
                    var nodeToMove = nodes[largestIndex];

                    int increment = Math.Max(0, nodeToMove.RowPos + largestForce) > nodeToMove.RowPos ? 1 : -1;
                    var targetPos = nodeToMove.RowPos + increment;

                    foreach (var test in nodes)
                        if (test.RowPos == targetPos)
                            test.RowPos -= increment;

                    nodeToMove.RowPos = targetPos;

                    nodes.Sort();
                }

                if (!anyChange)
                    break;
            }
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
        }
    }
}
