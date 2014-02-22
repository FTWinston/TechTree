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
        private static float nodeWidth, nodeHeight, depthIncrement, insetIncrement;
        public static Bitmap WriteImage(TechTree tree, int width, int height)
        {
            nodeWidth = Math.Min(200, width * 0.66666667f / (tree.MaxTreeColumn + 0.675f)); nodeHeight = Math.Min(nodeWidth / 1.41421356f, height * 0.5f / (tree.MaxTreeRow + 0.5f));
            depthIncrement = nodeHeight * 2f; insetIncrement = nodeWidth * 1.5f;

            Pen unlockPen = new Pen(Color.Black, height / 300f);
            Pen upgradePen1 = new Pen(Color.DarkGray, height / 80f);
            Pen upgradePen2 = new Pen(Color.Transparent, height / 300f);

            Bitmap image = new Bitmap(width, height);
            Font buildingFont = new Font(FontFamily.GenericSansSerif, nodeHeight / 5);
            StringFormat centered = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

            using (Graphics g = Graphics.FromImage(image))
            {
                var outline = new Pen(Color.Black, 2.5f);
                foreach (var building in tree.AllBuildings)
                {
                    float centerX, centerY;
                    var bounds = GetBounds(building, out centerX, out centerY);

                    foreach (var child in building.Unlocks)
                    {
                        float childX, childY;
                        GetBounds(child, out childX, out childY);
                        g.DrawLine(unlockPen, centerX, centerY, childX, childY);
                    }

                    foreach (var child in building.UpgradesTo)
                    {
                        float childX, childY;
                        GetBounds(child, out childX, out childY);
                        g.DrawLine(upgradePen1, centerX, centerY, childX, childY);
                        g.DrawLine(upgradePen2, centerX, centerY, childX, childY);
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
#if DEBUG
                Pen graphPen = new Pen(Color.Blue);
                float maxEnergy = (float)tree.Annealing.Max();
                float graphHeight = image.Height * 0.2f;
                double step = 1.0 * image.Width / tree.Annealing.Count;
                for (int i = 0; i < tree.Annealing.Count; i++)
                {
                    float x = (float)(i * step);
                    float y = (float)(tree.Annealing[i] / maxEnergy * graphHeight);

                    g.DrawLine(graphPen, new PointF(x, image.Height), new PointF(x, image.Height - y));
                }
                g.DrawLine(new Pen(Color.Red), new PointF((float)(tree.StepWhereBestFound * step), image.Height), new PointF((float)(tree.StepWhereBestFound * step), image.Height - 2));
#endif
            }

            return image;
        }

        private static RectangleF GetBounds(BuildingInfo building, out float xCenter, out float yCenter)
        {
            var bounds = new RectangleF(building.TreeColumn * insetIncrement, building.TreeRow * depthIncrement, nodeWidth, nodeHeight);
            xCenter = bounds.X + bounds.Width / 2f; yCenter = bounds.Y + bounds.Height / 2f;
            return bounds;
        }
    }
}
