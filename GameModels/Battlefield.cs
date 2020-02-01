using GameModels.Instances;
using Newtonsoft.Json;
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

        public Cell GetNeighbor(Cell from, Direction towards)
        {
            return GetNeighbor(from, (int)towards);
        }

        private Cell GetNeighbor(Cell from, int towards)
        {
            var offset = Offsets[towards];
            return GetCell(from.Col + offset.Col, from.Row + offset.Row);
        }

        private class Offset
        {
            public Offset(int col, int row) { Col = col; Row = row; }
            public int Col, Row;
        }

        private static readonly Offset[] Offsets = new Offset[] {
            new Offset(1, 0), // east
            new Offset(+1, -1), // northeast
            new Offset(0, -1), // northwest
            new Offset(-1, 0), // west
            new Offset(-1, +1), // southwest
            new Offset(0, +1), // southeast

            new Offset(2, -1), // diagonal northeast
            new Offset(1, -2), // diagonal north
            new Offset(-1, -1), // diagonal northwest
            new Offset(-2, 1), // diagonal southwest
            new Offset(-1, 2), // diagonal south
            new Offset(1, 1), // diagonal southwest
        };

        public enum Direction
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

        const int NumDirections = 6;

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

        public HashSet<Cell> GetReachableCells(Cell from, int moveDistance)
        {
            var visited = new HashSet<Cell>();
            visited.Add(from);

            var fringes = new List<List<Cell>>();
            var startList = new List<Cell>();
            startList.Add(from);
            fringes.Add(startList);

            for (var k = 1; k < moveDistance; k++)
            {
                var fringe = new List<Cell>();
                fringes.Add(fringe);
                foreach (var test in fringes[k-1])
                    for (var dir=0; dir<NumDirections; dir++)
                    {
                        var neighbor = GetNeighbor(test, dir);
                        if (neighbor == null || !neighbor.IsPassable || visited.Contains(neighbor))
                            continue;

                        visited.Add(neighbor);
                        fringe.Add(neighbor);
                    }
            }
            return visited;
        }
    }
}
