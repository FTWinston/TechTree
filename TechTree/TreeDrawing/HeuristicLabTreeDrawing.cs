using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views
{
    /*
    public interface ISymbolicExpressionTreeNode
    {
        int SubtreeCount { get; }

        ISymbolicExpressionTree GetSubtree(int i);
    }

    public interface ISymbolicExpressionTree
    {
        ISymbolicExpressionTreeNode[] IterateNodesBreadth();
    }

    public class Node
    {
        public Node Thread;
        public Node Ancestor;

        public float Mod; // position modifier
        public float Prelim;
        public float Change;
        public float Shift;
        public int Number;

        public float X;
        public float Y;

        public ISymbolicExpressionTreeNode SymbolicExpressionTreeNode;

        public bool IsLeaf
        {
            get { return SymbolicExpressionTreeNode.SubtreeCount == 0; }
        }
    }

    public class TreeLayout
    {
        private float distance = 5;
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        private ISymbolicExpressionTree symbolicExpressionTree;
        public ISymbolicExpressionTree SymbolicExpressionTree
        {
            get { return symbolicExpressionTree; }
            set
            {
                symbolicExpressionTree = value;
                nodes.Clear();
                var treeNodes = SymbolicExpressionTree.IterateNodesBreadth().ToList();
                foreach (var treeNode in treeNodes)
                {
                    var node = new Node { SymbolicExpressionTreeNode = treeNode };
                    node.Ancestor = node;
                    nodes.Add(treeNode, node);
                }
                // assign a number to each node, representing its position among its siblings (parent.IndexOfSubtree)
                foreach (var treeNode in treeNodes.Where(x => x.SubtreeCount > 0))
                {
                    for (int i = 0; i != treeNode.SubtreeCount; ++i)
                    {
                        nodes[treeNode.GetSubtree(i)].Number = i;
                    }
                }
                var r = nodes[symbolicExpressionTree.Root];
                FirstWalk(r);
                SecondWalk(r, -r.Prelim);
                NormalizeCoordinates();
            }
        }

        /// <summary>
        /// Returns a map of coordinates for each node in the symbolic expression tree.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ISymbolicExpressionTreeNode, PointF> GetNodeCoordinates()
        {
            var dict = new Dictionary<ISymbolicExpressionTreeNode, PointF>();
            if (nodes == null || nodes.Count == 0) return dict;
            foreach (var node in nodes.Values)
            {
                dict.Add(node.SymbolicExpressionTreeNode, new PointF { X = node.X, Y = node.Y });
            }
            return dict;
        }

        /// <summary>
        /// Returns the bounding box for this layout. When the layout is normalized, the rectangle should be [0,0,xmin,xmax].
        /// </summary>
        /// <returns></returns>
        public RectangleF Bounds()
        {
            float xmin, xmax, ymin, ymax; xmin = xmax = ymin = ymax = 0;
            var list = nodes.Values.ToList();
            for (int i = 0; i != list.Count; ++i)
            {
                float x = list[i].X, y = list[i].Y;
                if (xmin > x) xmin = x;
                if (xmax < x) xmax = x;
                if (ymin > y) ymin = y;
                if (ymax < y) ymax = y;
            }
            return new RectangleF(xmin, ymin, xmax + distance, ymax + distance);
        }

        /// <summary>
        /// Returns a string containing all the coordinates (useful for debugging).
        /// </summary>
        /// <returns></returns>
        public string DumpCoordinates()
        {
            if (nodes == null || nodes.Count == 0) return string.Empty;
            return nodes.Values.Aggregate("", (current, node) => current + (node.X + " " + node.Y + Environment.NewLine));
        }

        private readonly Dictionary<ISymbolicExpressionTreeNode, Node> nodes;

        public TreeLayout()
        {
            nodes = new Dictionary<ISymbolicExpressionTreeNode, Node>();
        }

        /// <summary>
        /// Transform node coordinates so that all coordinates are positive and start from 0.
        /// </summary>
        private void NormalizeCoordinates()
        {
            var list = nodes.Values.ToList();
            float xmin = 0, ymin = 0;
            for (int i = 0; i != list.Count; ++i)
            {
                if (xmin > list[i].X) xmin = list[i].X;
                if (ymin > list[i].Y) ymin = list[i].Y;
            }
            for (int i = 0; i != list.Count; ++i)
            {
                list[i].X -= xmin;
                list[i].Y -= ymin;
            }
        }

        private void FirstWalk(Node v)
        {
            Node w;
            if (v.IsLeaf)
            {
                w = LeftSibling(v);
                if (w != null)
                {
                    v.Prelim = w.Prelim + distance;
                }
            }
            else
            {
                var symbExprNode = v.SymbolicExpressionTreeNode;
                var defaultAncestor = nodes[symbExprNode.GetSubtree(0)]; // let defaultAncestor be the leftmost child of v
                for (int i = 0; i != symbExprNode.SubtreeCount; ++i)
                {
                    var s = symbExprNode.GetSubtree(i);
                    w = nodes[s];
                    FirstWalk(w);
                    Apportion(w, ref defaultAncestor);
                }
                ExecuteShifts(v);
                int c = symbExprNode.SubtreeCount;
                var leftmost = nodes[symbExprNode.GetSubtree(0)];
                var rightmost = nodes[symbExprNode.GetSubtree(c - 1)];
                float midPoint = (leftmost.Prelim + rightmost.Prelim) / 2;
                w = LeftSibling(v);
                if (w != null)
                {
                    v.Prelim = w.Prelim + distance;
                    v.Mod = v.Prelim - midPoint;
                }
                else
                {
                    v.Prelim = midPoint;
                }
            }
        }

        private void SecondWalk(Node v, float m)
        {
            v.X = v.Prelim + m;
            v.Y = symbolicExpressionTree.Root.GetBranchLevel(v.SymbolicExpressionTreeNode) * distance;
            var symbExprNode = v.SymbolicExpressionTreeNode;
            foreach (var s in symbExprNode.Subtrees)
            {
                SecondWalk(nodes[s], m + v.Mod);
            }
        }

        private void Apportion(Node v, ref Node defaultAncestor)
        {
            var w = LeftSibling(v);
            if (w == null) return;
            Node vip = v;
            Node vop = v;
            Node vim = w;
            Node vom = LeftmostSibling(vip);

            float sip = vip.Mod;
            float sop = vop.Mod;
            float sim = vim.Mod;
            float som = vom.Mod;

            while (NextRight(vim) != null && NextLeft(vip) != null)
            {
                vim = NextRight(vim);
                vip = NextLeft(vip);
                vom = NextLeft(vom);
                vop = NextRight(vop);
                vop.Ancestor = v;
                float shift = (vim.Prelim + sim) - (vip.Prelim + sip) + distance;
                if (shift > 0)
                {
                    var ancestor = Ancestor(vim, v) ?? defaultAncestor;
                    MoveSubtree(ancestor, v, shift);
                    sip += shift;
                    sop += shift;
                }
                sim += vim.Mod;
                sip += vip.Mod;
                som += vom.Mod;
                sop += vop.Mod;
            }
            if (NextRight(vim) != null && NextRight(vop) == null)
            {
                vop.Thread = NextRight(vim);
                vop.Mod += (sim - sop);
            }
            if (NextLeft(vip) != null && NextLeft(vom) == null)
            {
                vom.Thread = NextLeft(vip);
                vom.Mod += (sip - som);
                defaultAncestor = v;
            }
        }

        private void MoveSubtree(Node wm, Node wp, float shift)
        {
            int subtrees = wp.Number - wm.Number;
            wp.Change -= shift / subtrees;
            wp.Shift += shift;
            wm.Change += shift / subtrees;
            wp.Prelim += shift;
            wp.Mod += shift;
        }

        private void ExecuteShifts(Node v)
        {
            if (v.IsLeaf) return;
            float shift = 0;
            float change = 0;
            for (int i = v.SymbolicExpressionTreeNode.SubtreeCount - 1; i >= 0; --i)
            {
                var subtree = v.SymbolicExpressionTreeNode.GetSubtree(i);
                var w = nodes[subtree];
                w.Prelim += shift;
                w.Mod += shift;
                change += w.Change;
                shift += (w.Shift + change);
            }
        }

        #region Helper functions
        private Node Ancestor(Node vi, Node v)
        {
            var ancestor = vi.Ancestor;
            return ancestor.SymbolicExpressionTreeNode.Parent == v.SymbolicExpressionTreeNode.Parent ? ancestor : null;
        }

        private Node NextLeft(Node v)
        {
            int c = v.SymbolicExpressionTreeNode.SubtreeCount;
            return c == 0 ? v.Thread : nodes[v.SymbolicExpressionTreeNode.GetSubtree(0)]; // return leftmost child
        }

        private Node NextRight(Node v)
        {
            int c = v.SymbolicExpressionTreeNode.SubtreeCount;
            return c == 0 ? v.Thread : nodes[v.SymbolicExpressionTreeNode.GetSubtree(c - 1)]; // return rightmost child
        }

        private Node LeftSibling(Node n)
        {
            var parent = n.SymbolicExpressionTreeNode.Parent;
            if (parent == null) return null;
            int i = parent.IndexOfSubtree(n.SymbolicExpressionTreeNode);
            if (i == 0) return null;
            return nodes[parent.GetSubtree(i - 1)];
        }

        private Node LeftmostSibling(Node n)
        {
            var parent = n.SymbolicExpressionTreeNode.Parent;
            if (parent == null) return null;
            int i = parent.IndexOfSubtree(n.SymbolicExpressionTreeNode);
            if (i == 0) return null;
            return nodes[parent.GetSubtree(0)];
        }
        #endregion
    }*/
}