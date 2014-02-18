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
            float nodeWidth = Math.Min(200, width * 0.5f / (tree.MaxTreeColumn + 0.5f)), nodeHeight = Math.Min(nodeWidth / 1.41421356f, height * 0.5f / (tree.MaxTreeRow + 0.5f));
            float depthIncrement = nodeHeight * 2f, insetIncrement = nodeWidth * 2f;

            Pen linkPen = new Pen(Color.Black, height / 300f);
            Bitmap image = new Bitmap(width, height);
            Font buildingFont = new Font(FontFamily.GenericSansSerif, nodeHeight / 5);
            StringFormat centered = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

            using (Graphics g = Graphics.FromImage(image))
            {

                var outline = new Pen(Color.Black, 2.5f);
                foreach (var building in tree.AllBuildings)
                {
                    var bounds = new RectangleF(building.TreeColumn * insetIncrement, building.TreeRow * depthIncrement, nodeWidth, nodeHeight);
                    float xCenter = bounds.X + nodeWidth / 2f, yCenter = bounds.Y + nodeHeight / 2f;

                    foreach (var child in building.Unlocks)
                    {
                        g.DrawLine(linkPen, xCenter, yCenter,
                            child.TreeColumn * insetIncrement + nodeWidth / 2f,
                            child.TreeRow * depthIncrement + nodeHeight / 2f);
                    }

                    switch (building.Type)
                    {
                        case BuildingInfo.BuildingType.Factory: // rectangle
                            g.FillRectangle(new SolidBrush(building.TreeColor), bounds.X, bounds.Y, nodeWidth, nodeHeight);
                            g.DrawRectangle(outline, bounds.X, bounds.Y, nodeWidth, nodeHeight);
                            break;

                        case BuildingInfo.BuildingType.Tech: // octagon
                        case BuildingInfo.BuildingType.Resource:
                            var cornerFraction = 0.2f;
                            var path = new GraphicsPath();
                            path.AddLines(new PointF[] {
                                new PointF(bounds.X + nodeWidth * cornerFraction, bounds.Y),
                                new PointF(bounds.X + nodeWidth * (1 - cornerFraction), bounds.Y),
                                new PointF(bounds.X + nodeWidth, bounds.Y + nodeHeight * cornerFraction),
                                new PointF(bounds.X + nodeWidth, bounds.Y + nodeHeight * (1 - cornerFraction)),
                                new PointF(bounds.X + nodeWidth * (1 - cornerFraction), bounds.Y + nodeHeight),
                                new PointF(bounds.X + nodeWidth * cornerFraction, bounds.Y + nodeHeight),
                                new PointF(bounds.X, bounds.Y + nodeHeight * (1 - cornerFraction)),
                                new PointF(bounds.X, bounds.Y + nodeHeight * cornerFraction),
                                new PointF(bounds.X + nodeWidth * cornerFraction, bounds.Y)
                            });

                            g.FillPath(new SolidBrush(building.TreeColor), path);
                            g.DrawPath(outline, path);
                            break;

                        case BuildingInfo.BuildingType.Defense: // ellipse
                            g.FillEllipse(new SolidBrush(building.TreeColor), bounds.X, bounds.Y, nodeWidth, nodeHeight);
                            g.DrawEllipse(outline, bounds.X, bounds.Y, nodeWidth, nodeHeight);
                            break;
                    }

                    g.DrawString(building.Name, buildingFont, Brushes.Black, bounds, centered);
                }
            }
            return image;
        }
    }
}
