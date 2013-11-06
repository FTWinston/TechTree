using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TreeViewer
{
    static class TreeBitmapWriter
    {
        public static Bitmap WriteImage(TechTree.TechTree tree, int width, int height)
        {
            Bitmap image = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillRectangle(new SolidBrush(Color.Red), 0, 0, width, height);
            }
            return image;
        }
    }
}
