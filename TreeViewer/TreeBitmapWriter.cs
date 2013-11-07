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
                foreach (var node in sortedNodes)
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

        private static void RearrangeNodes(List<NodeHolder> sortedNodes, out int maxBreadth)
        {
            var integrator = new TreeIntegrator();
            sortedNodes = integrator.Run(sortedNodes, 1000, 0);

            // if there are blanks on the front of every depth, remove them
            // also determine the max row pos of any node

            int minRowPos = int.MaxValue;
            foreach (NodeHolder node in sortedNodes)
                minRowPos = Math.Min(minRowPos, node.RowPos);

            int depth = 0; maxBreadth = 0;
            foreach (NodeHolder node in sortedNodes)
            {
                node.RowPos -= minRowPos;

                if (node.Depth != depth)
                {
                    depth = node.Depth;
                    maxBreadth = Math.Max(maxBreadth, node.RowPos);
                }
            }
        }

        private static List<NodeHolder> SortByDepth(TechTree.TechTree tree)
        {
            var nodesByDepth = new SortedList<int, int>();
            var processedNodes = new List<NodeHolder>();
            var toProcess = new List<TreeNode>();
            toProcess.Add(tree.RootNode);

            while (toProcess.Count > 0)
            {
                TreeNode node = toProcess[0]; toProcess.RemoveAt(0);

                int depth = Depth(node);
                int numAtDepth;
                if (!nodesByDepth.TryGetValue(depth, out numAtDepth))
                    numAtDepth = 0;

                var nodeHolder = new NodeHolder(node, depth, numAtDepth * 2);
                foreach (var parent in node.Prerequisites)
                    nodeHolder.AddLink(parent, processedNodes);

                nodesByDepth[depth] = ++numAtDepth;
                processedNodes.Add(nodeHolder);

                foreach (TreeNode child in node.Unlocks)
                    if (!toProcess.Contains(child))
                        toProcess.Add(child);
            }

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

        private class TreeIntegrator : SimulatedAnnealing<List<NodeHolder>>
        {
            private List<NodeHolder> Clone(List<NodeHolder> state)
            {
                var newState = new List<NodeHolder>();

                foreach (var holder in state)
                    newState.Add(new NodeHolder(holder.Node, holder.Depth, holder.RowPos));

                return newState;
            }

            public override List<NodeHolder> SelectNeighbour(List<NodeHolder> state)
            {
                var newState = Clone(state);

                if (Random.NextDouble() < 0.667)
                {// Swap a pair (on the same depth), along with all their child nodes
                 // one might be a "blank" ... but there's no point in swapping two blanks.
                    throw new NotImplementedException();
                }
                else
                {
                    if (Random.NextDouble() < 0.3)
                    {// see if we have any blank nodes
                        int numBlanks = 0;

                        int lastDepth = 0, lastPos = -1;
                        foreach (var node in newState)
                        {
                            if (node.Depth == lastDepth)
                            {
                                numBlanks += node.RowPos - lastPos;
                            }
                            else
                            {
                                lastDepth = node.Depth;
                                numBlanks += node.RowPos;
                            }
                            lastPos = node.RowPos;
                        }

                        if (numBlanks > 0)
                        {// we have blanks, so remove one
                            int blankToRemove = Random.Next(numBlanks), currentBlank = -1;
                            lastDepth = 0; lastPos = -1;

                            for (int i = 0; i < newState.Count; i++)
                            {
                                var node = newState[i];
                                if (node.Depth == lastDepth)
                                {
                                    currentBlank += node.RowPos - lastPos;
                                }
                                else
                                {
                                    lastDepth = node.Depth;
                                    currentBlank += node.RowPos;
                                }
                                lastPos = node.RowPos;

                                if (currentBlank >= blankToRemove)
                                {
                                    for (int j = i; j < newState.Count; j++)
                                    {
                                        var node2 = newState[j];
                                        if (node2.Depth == lastDepth)
                                            node2.RowPos--;
                                        else
                                            break;
                                    }
                                    return newState;
                                }
                            }
                        }
                    }

                    // insert a blank node in somewhere, picked randomly
                    int pos = Random.Next(newState.Count), depth = newState[pos].Depth;
                    for (int i = pos; i < newState.Count; i++)
                    {
                        var node = newState[i];
                        if (node.Depth == depth)
                            node.RowPos++;
                        else
                            break;
                    }
                }

                return newState;
            }

            public override double DetermineEnergy(List<NodeHolder> state)
            {
                double energy = 0;

                // this currently ONLY considers the angles of connections, and not whether any overlap other nodes
                // that's probably ok for a pure tree, but isn't sufficient when nodes have multiple parents.

                foreach (var node in state)
                    foreach (var child in node.ChildLinks)
                        energy += DetermineEnergyComponent(child.Depth - node.Depth, Math.Abs(child.RowPos - node.RowPos));

                return energy;
            }

            private double DetermineEnergyComponent(double depthDiff, double posDiff)
            {
                return posDiff / depthDiff;
            }
        }
    }
}
