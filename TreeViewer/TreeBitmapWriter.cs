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

                    foreach (var oldLink in oldNode.ChildLinks)
                    {
                        newNode.ChildLinks.Add(FindNode(newState, oldLink.Node));
                    }
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

            private void DetermineDepthExtent(List<NodeHolder> state, int index, out int minIndex, out int maxIndex)
            {
                int depth = state[index].Depth;
                minIndex = maxIndex = index;
                while (maxIndex < state.Count - 1 && state[maxIndex + 1].Depth == depth)
                    maxIndex++;
                while (minIndex > 0 && state[minIndex - 1].Depth == depth)
                    minIndex--;
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
                    {// remove a blank node, if we have one
                        if (RemoveBlank(newState))
                            return newState;
                    }

                    // insert a blank node in somewhere, picked randomly
                    InsertBlank(newState);
                }

                return newState;
            }

            
            private void SwapTwoNodes(List<NodeHolder> state)
            {
                int firstNodeIndex = Random.Next(state.Count), minIndex, maxIndex, nodesVisited = 0;
                NodeHolder firstNode;
                while (true)
                {
                    DetermineDepthExtent(state, firstNodeIndex, out minIndex, out maxIndex);
                    firstNode = state[firstNodeIndex];

                    // there's more than 1 node at this depth if minIndex != maxIndex. Alternatively, there's a blank at this depth if the RowPos != 0.
                    if (firstNode.RowPos != 0 || minIndex != maxIndex)
                        break;

                    // this is the only node on its level, which isn't any good. Try the next node.
                    if (++nodesVisited >= state.Count)
                        return; // looped through every node, can't find two nodes on the same level to swap, anywhere.

                    firstNodeIndex++;
                    if (firstNodeIndex >= state.Count)
                        firstNodeIndex = 0;
                }

                int maxPosAtDepth = firstNode.RowPos;
                for (int i = minIndex; i <= maxIndex; i++)
                    maxPosAtDepth = Math.Max(maxPosAtDepth, state[i].RowPos);
                
                if (maxPosAtDepth == 0)
                    return;

                // select the second pos to swap with
                int secondNodePos;
                do
                {
                    secondNodePos = Random.Next(maxPosAtDepth + 1);
                } while (secondNodePos == firstNode.RowPos);

                // now find if there's a node in this pos or not
                NodeHolder secondNode = null;
                for (int i = minIndex; i <= maxIndex; i++)
                {
                    secondNode = state[i];
                    if (secondNode.RowPos == secondNodePos)
                        break;
                    else
                        secondNode = null;
                }
                
                // now that the two node are selected, we need to swap them. For now, not swapping child nodes, or shunting up actual indices

                if (secondNode == null)
                {// just change the firstNode to be in the new position
                    firstNode.RowPos = secondNodePos;
                }
                else
                {// swap the two nodes' positions
                    int tmp = secondNode.RowPos;
                    secondNode.RowPos = firstNode.RowPos;
                    firstNode.RowPos = tmp;
                }
            }

            private void InsertBlank(List<NodeHolder> state)
            {
                int index = Random.Next(state.Count), minIndex, maxIndex;
                DetermineDepthExtent(state, index, out minIndex, out maxIndex);

                int pos = state[index].RowPos;
                for (int i = minIndex; i<=maxIndex; i++)
                {
                    var node = state[i];
                    if ( node.RowPos >= pos )
                        node.RowPos++;
                }
            }

            private bool RemoveBlank(List<NodeHolder> state)
            {
                // pick a node, ensure there's a blank at that depth
                int testNodeIndex = Random.Next(state.Count), minIndex, maxIndex, nodesVisited = 0;
                NodeHolder testNode;
                while (true)
                {
                    DetermineDepthExtent(state, testNodeIndex, out minIndex, out maxIndex);
                    testNode = state[testNodeIndex];

                    // there's a blank at this depth if there's any node with a RowPos > maxIndex - minIndex
                    if (testNode.RowPos > maxIndex - minIndex)
                        break;

                    // this is the only node on its level, which isn't any good. Try the next node.
                    if (++nodesVisited >= state.Count)
                        return false; // looped through every node, can't find two nodes on the same level to swap, anywhere.

                    testNodeIndex++;
                    if (testNodeIndex >= state.Count)
                        testNodeIndex = 0;
                }

                /*
                int minNodeIndex, maxNodeIndex, testNodeIndex = Random.Next(state.Count), nodesVisited = 0;
                NodeHolder testNode;
                while (true)
                {
                    testNode = state[testNodeIndex];

                    // there must be a blank at the same depth
                    if (testNode.RowPos > 0)
                        break;
                    if (testNodeIndex < state.Count - 1 && state[testNodeIndex + 1].Depth == testNode.Depth)
                        break;
                    if (testNodeIndex > 0 && state[testNodeIndex - 1].Depth == testNode.Depth)
                        break;

                    // this is the only node on its level, which isn't any good. Try the next node.
                    if (++nodesVisited >= state.Count)
                        return false; // looped through every node, can't find two nodes on the same level to swap, anywhere.

                    testNodeIndex++;
                    if (testNodeIndex >= state.Count)
                        testNodeIndex = 0;
                }

                // determine the extent of all nodes at this depth
                minNodeIndex = maxNodeIndex = testNodeIndex;
                int maxPosAtDepth = testNode.RowPos;
                while (maxNodeIndex < state.Count - 1 && state[maxNodeIndex + 1].Depth == testNode.Depth)
                {
                    maxNodeIndex++;
                    maxPosAtDepth = Math.Max(maxPosAtDepth, state[maxNodeIndex].RowPos);
                }
                while (minNodeIndex > 0 && state[minNodeIndex - 1].Depth == testNode.Depth)
                {
                    minNodeIndex--;
                    maxPosAtDepth = Math.Max(maxPosAtDepth, state[minNodeIndex].RowPos);
                }


                int numBlanks = 0, lastDepth = -1, lastPos = 0;
                foreach (var node in state)
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



                int blankToRemove = Random.Next(numBlanks), currentBlank = -1;
                lastDepth = -1; lastPos = 0;

                for (int i = 0; i < state.Count; i++)
                {
                    var node = state[i];
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
                        for (int j = i; j < state.Count; j++)
                        {
                            var node2 = state[j];
                            if (node2.Depth == lastDepth)
                                node2.RowPos--;
                            else
                                break;
                        }
                        return false;
                    }
                }

                return true;*/
                return false;
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
