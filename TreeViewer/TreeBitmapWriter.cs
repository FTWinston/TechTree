using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TechTree;

namespace TreeViewer
{
    static class TreeBitmapWriter
    {
        public static Bitmap WriteImage(TechTree.TechTree tree, int width, int height)
        {
            var sortedNodes = SortByDepth(tree);

            int depth = sortedNodes.Count, breadth;
            RearrangeNodes(sortedNodes, out breadth);

            Brush nodeBrush = new SolidBrush(Color.DarkGray);
            Pen linkPen = new Pen(Color.Black, height / 300f);
            Bitmap image = new Bitmap(width, height);

            float nodeWidth = width * 0.5f / (depth - 0.5f), nodeHeight = Math.Min(nodeWidth / 1.41421356f, height * 1f / breadth);
            float depthIncrement = nodeWidth * 2f, insetIncrement = nodeHeight;

            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var list in sortedNodes)
                    foreach (var node in list.Value)
                    {
                        float xpos = node.Depth * depthIncrement, xCenter = xpos + nodeWidth / 2f;
                        float ypos = node.RowPos * insetIncrement, yCenter = ypos + nodeHeight / 2f;

                        foreach (var child in node.ChildLinks)
                        {
                            g.DrawLine(linkPen, xCenter, yCenter,
                                child.Depth * depthIncrement + nodeWidth / 2f,
                                child.RowPos * insetIncrement + nodeHeight / 2f);
                        }

                        g.FillEllipse(nodeBrush, xpos, ypos, nodeWidth, nodeHeight);
                    }
            }
            return image;
        }

        private static void RearrangeNodes(SortedList<int, List<NodeHolder>> sortedNodes, out int maxBreadth)
        {
            // we need a fitness function to decide how elegant the overall thing is:
            //  Node lines passing too close to other nodes (e.g. through them) is very bad
            //  Needless "angled" connections is bad - child nodes generally want to be in-line with their parent

            // we then want to run a GA (I don't think regular hill-climbing will cut it). Mutations are:
            //  Swapping a pair (on the same depth), along with all their child nodes
            //  Inserting a blank node
            //  Removing a blank node

            maxBreadth = 1;
            for (int depth = 1; depth < sortedNodes.Count; depth++)
            {
                var nodesAtDepth = sortedNodes[depth];
                maxBreadth = Math.Max(maxBreadth, nodesAtDepth.Last().RowPos + 1);
            }

            // the root node should be centered, vertically
            sortedNodes[0][0].RowPos = maxBreadth / 2;
        }

        private static SortedList<int, List<NodeHolder>> SortByDepth(TechTree.TechTree tree)
        {
            var nodesByDepth = new SortedList<int, List<NodeHolder>>();
            var processedNodes = new List<NodeHolder>();
            var toProcess = new List<TreeNode>();
            toProcess.Add(tree.RootNode);

            while (toProcess.Count > 0)
            {
                TreeNode node = toProcess[0]; toProcess.RemoveAt(0);

                int depth = Depth(node);
                List<NodeHolder> nodesAtDepth;
                if (!nodesByDepth.TryGetValue(depth, out nodesAtDepth))
                {
                    nodesAtDepth = new List<NodeHolder>();
                    nodesByDepth[depth] = nodesAtDepth;
                }

                var nodeHolder = new NodeHolder(node, depth, nodesAtDepth.Count * 2);
                foreach (var parent in node.Prerequisites)
                    nodeHolder.AddLink(parent, processedNodes);

                nodesAtDepth.Add(nodeHolder);
                processedNodes.Add(nodeHolder);

                foreach (TreeNode child in node.Unlocks)
                    if (!toProcess.Contains(child))
                        toProcess.Add(child);
            }

            return nodesByDepth;
        }

        private static int Depth(TreeNode node)
        {
            if ( node.Prerequisites.Count == 0 )
                return 0;
            
            int depth = 0;
            foreach (TreeNode parent in node.Prerequisites)
                depth = Math.Max(depth, Depth(parent));
            return depth + 1;
        }

        private class NodeHolder
        {
            public TreeNode Node { get; private set; }
            public int Depth { get; private set; }
            public int RowPos { get; set; }
            public List<NodeHolder> ChildLinks = new List<NodeHolder>();

            public NodeHolder(TreeNode node, int depth, int rowPos)
            {
                Node = node;
                Depth = depth;
                RowPos = rowPos;
            }

            public void AddLink(TreeNode parent, List<NodeHolder> nodes)
            {
                var parentHolder = FindNodeHolder(parent, nodes);
                parentHolder.ChildLinks.Add(this);
            }

            NodeHolder FindNodeHolder(TreeNode node, List<NodeHolder> nodes)
            {
                foreach (NodeHolder test in nodes)
                    if (test.Node == node)
                        return test;
                return null;
            }
        }
    }
}
