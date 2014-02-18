using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GameLogic;
using System.Drawing.Drawing2D;

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
                foreach (var building in tree.AllBuildings)
                {
                    float xpos = building.TreeColumn * insetIncrement, xCenter = xpos + nodeWidth / 2f;
                    float ypos = building.TreeRow * depthIncrement, yCenter = ypos + nodeHeight / 2f;

                    foreach (var child in building.Unlocks)
                    {
                        g.DrawLine(linkPen, xCenter, yCenter,
                            child.TreeColumn * insetIncrement + nodeWidth / 2f,
                            child.TreeRow * depthIncrement + nodeHeight / 2f);
                    }

                    switch (building.Type)
                    {
                        case BuildingInfo.BuildingType.Factory: // rectangle
                            g.FillRectangle(new SolidBrush(building.TreeColor), xpos, ypos, nodeWidth, nodeHeight);
                            break;

                        case BuildingInfo.BuildingType.Tech: // octagon
                            var cornerFraction = 0.2f;
                            var path = new GraphicsPath();
                            path.AddLines(new PointF[] {
                                new PointF(xpos + nodeWidth * cornerFraction, ypos),
                                new PointF(xpos + nodeWidth * (1 - cornerFraction), ypos),
                                new PointF(xpos + nodeWidth, ypos + nodeHeight * cornerFraction),
                                new PointF(xpos + nodeWidth, ypos + nodeHeight * (1 - cornerFraction)),
                                new PointF(xpos + nodeWidth * (1 - cornerFraction), ypos + nodeHeight),
                                new PointF(xpos + nodeWidth * cornerFraction, ypos + nodeHeight),
                                new PointF(xpos, ypos + nodeHeight * (1 - cornerFraction)),
                                new PointF(xpos, ypos + nodeHeight * cornerFraction),
                            });

                            g.FillPath(new SolidBrush(building.TreeColor), path);
                            break;

                        case BuildingInfo.BuildingType.Utility: // ellipse
                            g.FillEllipse(new SolidBrush(building.TreeColor), xpos, ypos, nodeWidth, nodeHeight);
                            break;
                    }
                }
            }
            return image;
        }
    }
}
