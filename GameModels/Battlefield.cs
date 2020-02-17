using GameModels.Instances;
using System;
using System.Collections.Generic;

namespace GameModels
{
    public class Battlefield
    {
        public Battlefield(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width * height];
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Cell[] Cells { get; private set; }

        public List<int> StartPositions { get; } = new List<int>();

        public int GetIndex(int col, int row)
        {
            return col + row * Width;
        }

        private Cell GetCell(int col, int row)
        {
            int index = GetIndex(col, row);

            return index < 0 || index >= Cells.Length
                ? null
                : Cells[index];
        }

        private class Offset
        {
            public Offset(int col, int row)
            {
                Col = col;
                Row = row;
            }

            public int Col { get; }
            public int Row { get; }
        }

        private static readonly Offset[] CardinalOffsets =
        {
            new Offset(1, 0), // east
            new Offset(+1, -1), // northeast
            new Offset(0, -1), // northwest
            new Offset(-1, 0), // west
            new Offset(-1, +1), // southwest
            new Offset(0, +1), // southeast
        };

        public IEnumerable<Cell> GetNeighbours(Cell from)
        {
            foreach (var offset in CardinalOffsets)
            {
                var neighbour = GetCell(from.Col + offset.Col, from.Row + offset.Row);

                if (neighbour != null)
                    yield return neighbour;
            }
        }

        public int GetDistance(Cell from, Cell to)
        {
            return (
                Math.Abs(from.Col - to.Col)
              + Math.Abs(from.Col + from.Row - to.Col - to.Row)
              + Math.Abs(from.Row - to.Row)
            ) / 2;
        }

        public HashSet<Cell> GetCellsWithinDistance(Cell from, int distance)
        {
            var cells = new HashSet<Cell>();
            for (var dx = -distance; dx <= distance; dx++)
            {
                var minZ = Math.Max(-distance, -dx - distance);
                var maxZ = Math.Min(distance, -dx + distance);
                for (var dz = minZ; dz <= maxZ; dz++)
                    cells.Add(GetCell(from.Col + dx, from.Row + dz));
            }
            return cells;
        }
    }
}
