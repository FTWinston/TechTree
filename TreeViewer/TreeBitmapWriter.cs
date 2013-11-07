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
            Brush nodeBrush = new SolidBrush(Color.DarkGray);
            Bitmap image = new Bitmap(width, height);

            int maxBreadth = 1;
            foreach (var nodeList in sortedNodes)
                maxBreadth = Math.Max(nodeList.Value.Count, maxBreadth);

            float nodeWidth = width * 0.5f / (sortedNodes.Count - 0.5f), nodeHeight = Math.Min(nodeWidth / 1.41421356f, height * 0.5f / (maxBreadth - 0.5f));
            float depthIncrement = nodeWidth * 2f, insetIncrement = nodeHeight * 2f;

            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var nodeList in sortedNodes)
                {
                    int i = 0;
                    foreach (var node in nodeList.Value)
                    {
                        g.FillEllipse(nodeBrush, nodeList.Key * depthIncrement, i * insetIncrement, nodeWidth, nodeHeight);
                        i++;
                    }
                }
            }
            return image;
        }

        private static SortedList<int, List<TreeNode>> SortByDepth(TechTree.TechTree tree)
        {
            var nodesByDepth = new SortedList<int, List<TreeNode>>();

            List<TreeNode> toProcess = new List<TreeNode>();
            toProcess.Add(tree.RootNode);

            while (toProcess.Count > 0)
            {
                TreeNode node = toProcess[0]; toProcess.RemoveAt(0);

                int depth = Depth(node);
                List<TreeNode> nodesAtDepth;
                if (!nodesByDepth.TryGetValue(depth, out nodesAtDepth))
                {
                    nodesAtDepth = new List<TreeNode>();
                    nodesByDepth.Add(depth, nodesAtDepth);
                }
                nodesAtDepth.Add(node);

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
    }
}
