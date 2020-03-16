using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Map
{
    public class HexMap<TCell> : IGraph<TCell>
        where TCell : HexCell
    {
        public HexMap(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new TCell[width * height];
        }

        public int Width { get; }
        public int Height { get; }
        public TCell?[] Cells { get; }

        public int GetIndex(int col, int row)
        {
            return col + row * Width;
        }

        public int GetRow(int index)
        {
            return index / Width;
        }

        public int GetCol(int index)
        {
            return index % Width;
        }

        private TCell? GetCell(int col, int row)
        {
            int index = GetIndex(col, row);

            return index < 0 || index >= Cells.Length
                ? null
                : Cells[index];
        }

        private TCell? GetNearestCell(float col, float row, float z)
        {
            int rCol = (int)Math.Round(col);
            int rRow = (int)Math.Round(row);
            int rz = (int)Math.Round(z);

            var colDiff = Math.Abs(rCol - col);
            var rowDiff = Math.Abs(rRow - row);
            var zDiff = Math.Abs(rz - z);

            if (colDiff > rowDiff && colDiff > zDiff)
                rCol = -rRow - rz;
            else if (rowDiff > zDiff)
                rRow = -rCol - rz;
            //else
            //    rz = -rx - ry;

            return GetCell(rCol, rRow);
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

        public IEnumerable<TCell> GetNeighbors(TCell from)
        {
            foreach (var offset in CardinalOffsets)
            {
                var neighbour = GetCell(from.Col + offset.Col, from.Row + offset.Row);

                if (neighbour != null)
                    yield return neighbour;
            }
        }

        public int GetDistance(TCell from, TCell to)
        {
            return (
                Math.Abs(from.Col - to.Col)
              + Math.Abs(from.Col + from.Row - to.Col - to.Row)
              + Math.Abs(from.Row - to.Row)
            ) / 2;
        }

        public HashSet<TCell> GetCellsWithinDistance(TCell from, int distance)
        {
            var cells = new HashSet<TCell>();

            for (var dx = -distance; dx <= distance; dx++)
            {
                var minZ = Math.Max(-distance, -dx - distance);
                var maxZ = Math.Min(distance, -dx + distance);
                for (var dz = minZ; dz <= maxZ; dz++)
                {
                    var cell = GetCell(from.Col + dx, from.Row + dz);

                    if (cell != null)
                        cells.Add(cell);
                }
            }

            return cells;
        }


        public IEnumerable<TCell> TraceLine(TCell from, TCell to)
        {
            if (from == to)
            {
                yield return to;
                yield break;
            }

            int distance = GetDistance(from, to);

            // row + col + z = 0, so z = -row - col
            int fromZ = -from.Col - from.Row;
            int toZ = -to.Col - to.Row;

            for (var i = 1; i <= distance; i++)
            {
                var t = 1f / distance * i;

                var col = Lerp(from.Col, to.Col, t);
                var row = Lerp(from.Row, to.Row, t);
                var z = Lerp(fromZ, toZ, t);

                var cell = GetNearestCell(col, row, z);

                if (cell == null)
                    yield break;

                yield return cell;
            }
        }

        private float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
