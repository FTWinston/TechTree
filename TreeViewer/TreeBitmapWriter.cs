using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GameLogic;

namespace TreeViewer
{
    static class TreeBitmapWriter
    {
        public static Bitmap WriteImage(TechTree tree, int width, int height)
        {
            Brush nodeBrush = new SolidBrush(Color.DarkGray);
            Pen linkPen = new Pen(Color.Black, height / 300f);
            Bitmap image = new Bitmap(width, height);

            float nodeWidth = Math.Min(200, width * 0.5f / (tree.MaxTreeColumn + 0.5f)), nodeHeight = Math.Min(nodeWidth / 1.41421356f, height * 0.5f / (tree.MaxTreeRow + 0.5f));
            float depthIncrement = nodeHeight * 2f, insetIncrement = nodeWidth * 2f;

            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var node in tree.AllBuildings)
                {
                    float xpos = node.TreeColumn * insetIncrement, xCenter = xpos + nodeWidth / 2f;
                    float ypos = node.TreeRow * depthIncrement, yCenter = ypos + nodeHeight / 2f;

                    foreach (var child in node.Unlocks)
                    {
                        g.DrawLine(linkPen, xCenter, yCenter,
                            child.TreeColumn * insetIncrement + nodeWidth / 2f,
                            child.TreeRow * depthIncrement + nodeHeight / 2f);
                    }

                    g.FillEllipse(new SolidBrush(node.TreeColor), xpos, ypos, nodeWidth, nodeHeight);
                }
            }
            return image;
        }
    }
}
