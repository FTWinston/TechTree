using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Cell[] Cells { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width * height];
        }
    }
}
