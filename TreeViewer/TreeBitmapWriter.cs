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

                for (int i = 0; i < state.Count; i++)
                {
                    var newNode = newState[i]; var oldNode = state[i];

                    foreach (var link in oldNode.ChildLinks)
                        newNode.ChildLinks.Add(FindNode(newState, link.Node));
                }

                return newState;
            }

            private NodeHolder FindNode(List<NodeHolder> state, TreeNode node)
            {
                foreach (var holder in state)
                    if (holder.Node == node)
                        return holder;
                return null;
            }

            public override List<NodeHolder> SelectNeighbour(List<NodeHolder> state)
            {
                var newState = Clone(state);

                if (Random.NextDouble() < 0.667)
                {// Swap a pair (on the same depth)
                 // one might be a "blank" ... but there's no point in swapping two blanks.
                    SwapTwoNodes(newState);
                }
                else
                {
                    if (Random.NextDouble() < 0.3)
                    {// see if we have any blank nodes
                        int numBlanks = 0, lastDepth = -1, lastPos = 0;
                        foreach (var node in newState)
                        {
                            if (node.Depth == lastDepth)
                            {
                                numBlanks += node.RowPos - lastPos - 1;
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
                            RemoveBlank(newState, numBlanks);
                            return newState;
                        }
                    }

                    // insert a blank node in somewhere, picked randomly
                    InsertBlank(newState);
                }

                return newState;
            }

            private void SwapTwoNodes(List<NodeHolder> state)
            {
                int minNodeIndex, maxNodeIndex, firstNodeIndex = Random.Next(state.Count), nodesVisited = 0;
                NodeHolder firstNode;
                while (true)
                {
                    firstNode = state[firstNodeIndex];

                    // there must be another node (or a blank) at the same depth. If this node isn't first, then it's ok.
                    if (firstNode.RowPos > 0)
                        break;

                    // if another node follows it, then it's also ok
                    NodeHolder nextNode = state[firstNodeIndex < state.Count - 1 ? firstNodeIndex + 1 : 0];
                    if (nextNode.Depth == firstNode.Depth)
                        break;

                    // otherwise, this is the only one on its level, it's not good.
                    if (++nodesVisited >= state.Count)
                        return; // looped through every node, can't find two nodes on the same level to swap, anywhere.

                    firstNodeIndex++;
                    if (firstNodeIndex >= state.Count)
                        firstNodeIndex = 0;
                }

                // determine the extent of all nodes at this depth
                minNodeIndex = maxNodeIndex = firstNodeIndex;
                while (maxNodeIndex < state.Count - 1 && state[maxNodeIndex + 1].Depth == firstNode.Depth)
                    maxNodeIndex++;
                while (minNodeIndex > 0 && state[minNodeIndex - 1].Depth == firstNode.Depth)
                    minNodeIndex--;

                // select the second pos to swap with
                int secondNodePos;
                do
                {
                    secondNodePos = Random.Next(state[maxNodeIndex].RowPos + 1);
                } while (secondNodePos == firstNode.RowPos);

                // now find if there's a node in this pos or not
                NodeHolder secondNode = null; int secondNodeIndex = -1, testNodeIndex = firstNodeIndex;
                int increment = secondNodePos < firstNode.RowPos ? -1 : 1;

                while (testNodeIndex > minNodeIndex && testNodeIndex < maxNodeIndex)
                {
                    testNodeIndex += increment;
                    int pos = state[testNodeIndex].RowPos;

                    if (pos == secondNodePos)
                    {
                        secondNode = state[testNodeIndex];
                        secondNodeIndex = testNodeIndex;
                        break;
                    }
                    else if ((increment < 0 && pos < secondNodePos)
                        || (increment > 0 && pos > secondNodePos))
                        break;
                }

                // now that the two node are selected, we need to swap them. For now, not swapping child nodes or anything
                if (secondNode == null)
                {
                    // shunt the other nodes up/down (their actual indices, not their RowPoses)
                    if (increment < 0)
                    {// we want to insert AFTER testNodeIndex, and shuffle everything up to (and including) firstNodeIndex
                        for (int i = firstNodeIndex - 1; i > testNodeIndex; i--)
                            state[i + 1] = state[i];
                        if (firstNodeIndex != testNodeIndex + 1)
                            state[testNodeIndex + 1] = firstNode;
                    }
                    else
                    {// we want to insert BEFORE testNodeIndex, and shuffle everything from 1 before testNodeIndex down to firstNodeIndex
                        for (int i = firstNodeIndex + 1; i < testNodeIndex; i++)
                            state[i - 1] = state[i];
                        if (firstNodeIndex != testNodeIndex - 1)
                            state[testNodeIndex - 1] = firstNode;
                    }

                    firstNode.RowPos = secondNodePos;
                }
                else
                {// a straight up swap
                    state[firstNodeIndex] = secondNode;
                    state[secondNodeIndex] = firstNode;

                    int tmp = secondNode.RowPos;
                    secondNode.RowPos = firstNode.RowPos;
                    firstNode.RowPos = tmp;
                }
            }

            private void InsertBlank(List<NodeHolder> newState)
            {
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

            private void RemoveBlank(List<NodeHolder> newState, int numBlanks)
            {
                int blankToRemove = Random.Next(numBlanks), currentBlank = -1;
                int lastDepth = -1, lastPos = 0;

                for (int i = 0; i < newState.Count; i++)
                {
                    var node = newState[i];
                    if (node.Depth == lastDepth)
                    {
                        currentBlank += node.RowPos - lastPos - 1;
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
                        return;
                    }
                }
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
