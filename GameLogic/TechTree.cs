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
            bool anyMovement = false;
            while (CondenseLayout(RootNode, true))
                anyMovement = true;

            if (anyMovement)
            {
                int maxRowPos = 0;
                foreach (var node in AllNodes)
                    maxRowPos = Math.Max(maxRowPos, node.RowPos);

                MaxRowPos = maxRowPos;
            }

            anyMovement = false;
            while (CondenseLayout(RootNode, false))
                anyMovement = true;

            if (anyMovement)
            {
                int minRowPos = int.MaxValue;
                foreach (var node in AllNodes)
                    minRowPos = Math.Min(minRowPos, node.RowPos);

                if (minRowPos > 0)
                {
                    foreach (var node in AllNodes)
                        node.RowPos -= minRowPos;

                    MaxRowPos -= minRowPos;
                }
            }
        }

        private bool CondenseLayout(TreeNode node, bool leftward)
        {
            // if any of this node's descendents can have its entire sub-tree's rowpos reduced by 1, then do that.
            // that will make more efficient use of space.

            bool retVal = false;
            for (int i = leftward ? 0 : node.Unlocks.Count - 1; leftward ? i < node.Unlocks.Count : i >= 0; i += leftward ? 1 : -1)
            {
                var child = node.Unlocks[i];
                if (CanShiftRowPos(node, child, leftward) && (leftward ? child.RowPos > node.RowPos : child.RowPos < node.RowPos))
                {
                    ShiftRowPos(child, leftward);
                    retVal = true;
                }

                if (CondenseLayout(child, leftward))
                    retVal = true;
            }
            return retVal;
        }

        private bool CanShiftRowPos(TreeNode parent, TreeNode node, bool leftward)
        {
            if (leftward ? node.RowPos == 0 : node.RowPos == MaxRowPos)
                return false;
            int pos = AllNodes.IndexOf(node);
            TreeNode adjacent = AllNodes[leftward ? (pos > 0 ? pos - 1 : 0) : (pos < AllNodes.Count - 1 ? pos + 1 : 0)];
            if (adjacent.Depth == node.Depth && (leftward ? (adjacent.RowPos >= node.RowPos - 1) : (adjacent.RowPos <= node.RowPos + 1)))
                return false;

            if (node.Unlocks.Count > 0)
            {
                var endNodes = new SortedList<TreeNode, TreeNode>();
                if (leftward)
                    FindLeftmostAtEachLevel(node, endNodes);
                else
                {
                    int[] bestRowPos = new int[MaxDepth+1];
                    FindRightmostAtEachLevel(node, endNodes, bestRowPos);
                }

                // if the endmost child node at each level can shunt up, the rest are only shunting after it, anyway
                foreach (var kvp in endNodes)
                    if (!CanShiftRowPos(kvp.Key, kvp.Value, leftward))
                        return false;
            }
            
            return true;
        }

        private void FindLeftmostAtEachLevel(TreeNode node, SortedList<TreeNode, TreeNode> endNodes)
        {// This can be simplified greatly, because the bigger trees are always leftmost, so the leftmost nodes are ALWAYS the leftmost chain
            do
            {
                var end = node.Unlocks[0];
                endNodes.Add(node, end);

                node = end;
            } while (node.Unlocks.Count > 0);
        }

        private void FindRightmostAtEachLevel(TreeNode node, SortedList<TreeNode, TreeNode> endNodes, int[] bestRowPos)
        {
            var test = node.Unlocks[node.Unlocks.Count - 1];
            if (test.RowPos >= bestRowPos[test.Depth])
            {
                endNodes[node] = test;
                bestRowPos[test.Depth] = test.RowPos;
            }

            foreach (var child in node.Unlocks)
                if (child.Unlocks.Count > 0)
                    FindRightmostAtEachLevel(child, endNodes, bestRowPos);
        }

        private void ShiftRowPos(TreeNode node, bool leftward)
        {
            node.RowPos += leftward ? -1 : 1;
            foreach (var child in node.Unlocks)
                ShiftRowPos(child, leftward);
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
