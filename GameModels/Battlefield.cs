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

        private Cell GetNeighbour(Cell from, Direction towards)
        {
            var offset = Offsets[towards];
            return GetCell(from.Col + offset.Col, from.Row + offset.Row);
        }

        public IEnumerable<Cell> GetNeighbours(Cell from)
        {
            foreach (var direction in CardinalDirections)
            {
                var neighbour = GetNeighbour(from, direction);
                if (neighbour != null)
                    yield return neighbour;
            }
        }

        private enum Direction
        {
            East = 0,
            NorthEast = 1,
            NorthWest = 2,
            West = 3,
            SouthWest = 4,
            SouthEast = 5,

            Diagonal_NorthEast = 6,
            Diagonal_North = 7,
            Diagonal_NorthWest = 8,
            Diagonal_SouthWest = 9,
            Diagonal_South = 10,
            Diagonal_SouthEast = 11,
        }

        private static readonly Direction[] CardinalDirections =
        {
            Direction.East,
            Direction.NorthEast,
            Direction.NorthWest,
            Direction.West,
            Direction.SouthWest,
            Direction.SouthEast,
        };

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

        private static readonly Dictionary<Direction, Offset> Offsets = new Dictionary<Direction, Offset>
        {
            { Direction.East, new Offset(1, 0) },
            { Direction.NorthEast, new Offset(+1, -1) },
            { Direction.NorthWest, new Offset(0, -1) },
            { Direction.West, new Offset(-1, 0) },
            { Direction.SouthWest, new Offset(-1, +1) },
            { Direction.SouthEast, new Offset(0, +1) },

            { Direction.Diagonal_NorthEast, new Offset(2, -1) },
            { Direction.Diagonal_North, new Offset(1, -2) },
            { Direction.Diagonal_NorthWest, new Offset(-1, -1) },
            { Direction.Diagonal_SouthWest, new Offset(-2, 1) },
            { Direction.Diagonal_South, new Offset(-1, 2) },
            { Direction.Diagonal_SouthEast, new Offset(1, 1) },
        };

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
