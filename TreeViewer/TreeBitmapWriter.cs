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
            int depth, breadth;
            var sortedNodes = SortByDepth(tree, out depth, out breadth);
            Brush nodeBrush = new SolidBrush(Color.DarkGray);
            Pen linkPen = new Pen(Color.Black, height / 300f);
            Bitmap image = new Bitmap(width, height);

            float nodeWidth = width * 0.5f / (depth - 0.5f), nodeHeight = Math.Min(nodeWidth / 1.41421356f, height * 0.5f / (breadth - 0.5f));
            float depthIncrement = nodeWidth * 2f, insetIncrement = nodeHeight * 2f;

            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var node in sortedNodes)
                {
                    float xpos = node.Depth * depthIncrement, xCenter = xpos + nodeWidth/2f;
                    float ypos = node.Node == tree.RootNode ? (height-nodeHeight)/2f : node.RowPos * insetIncrement, yCenter = ypos + nodeHeight/2f;

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

        private static List<NodeHolder> SortByDepth(TechTree.TechTree tree, out int maxDepth, out int maxBreadth)
        {
            var nodesByDepth = new SortedList<int, int>();
            var processedNodes = new List<NodeHolder>();
            var toProcess = new List<TreeNode>();
            toProcess.Add(tree.RootNode);

            maxBreadth = 0;
            while (toProcess.Count > 0)
            {
                TreeNode node = toProcess[0]; toProcess.RemoveAt(0);

                int depth = Depth(node);
                int numAtDepth;
                if (!nodesByDepth.TryGetValue(depth, out numAtDepth))
                    numAtDepth = 0;
                
                var nodeHolder = new NodeHolder(node, depth, numAtDepth++);
                foreach (var parent in node.Prerequisites)
                    nodeHolder.AddLink(parent, processedNodes);
                nodesByDepth[depth] = numAtDepth;
                processedNodes.Add(nodeHolder);
                maxBreadth = Math.Max(maxBreadth, numAtDepth);

                foreach (TreeNode child in node.Unlocks)
                    if (!toProcess.Contains(child))
                        toProcess.Add(child);
            }

            maxDepth = nodesByDepth.Count;
            return processedNodes;
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
            public int RowPos { get; private set; }
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
