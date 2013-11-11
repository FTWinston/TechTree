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
                    int[] forces = new int[nodes.Count];
                    for (int i = 0; i < forces.Length; i++)
                    {
                        var node = nodes[i];
                        foreach (var parent in node.Prerequisites)
                            forces[i] += parent.RowPos - node.RowPos;

                        foreach (var child in node.Unlocks)
                            forces[i] += child.RowPos - node.RowPos;
                    }

                    // apply the calculated forces to rearrange the nodes
                    for (int i = 0; i < forces.Length; i++)
                    {
                        if (forces[i] == 0)
                            continue;
                        var node = nodes[i];

                        var targetPos = Math.Max(0, node.RowPos + forces[i]);
                        int increment = targetPos > node.RowPos ? 1 : -1;

                        for (int testPos = node.RowPos + increment; testPos != targetPos; testPos += increment)
                        {
                            TreeNode other = FindNodeAtPos(nodes, testPos);
                            if (other == null)
                            {// if there's a RowPos free, move into it.
                                node.RowPos = testPos;
                                anyChange = true;
                                break;
                            }
                            else if (testPos == node.RowPos + 1 && i < forces.Length - 1 && forces[i + 1] <= 0)
                            {// if there's a node immediately below that wants to move up (or wants to stay put), and we want to move down, swap, but ensure we won't get moved by ITS force
                                var tmp = node.RowPos;
                                node.RowPos = testPos;
                                other.RowPos = tmp;
                                nodes[i + 1].RowPos = tmp;
                                forces[i + 1] = 0;
                                anyChange = true;
                                break;
                            }
                        }

                        nodes.Sort();
                    }
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
