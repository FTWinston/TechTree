using GameModels.Instances;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameModels
{
    public class HexCell
    {
        public HexCell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        [JsonIgnore]
        public int Col { get; }

        [JsonIgnore]
        public int Row { get; }
    }

    public class HexMap<TCell>
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
        public TCell[] Cells { get; }

        public int GetIndex(int col, int row)
        {
            return col + row * Width;
        }

        private TCell GetCell(int col, int row)
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

        public IEnumerable<TCell> GetNeighbours(Cell from)
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
                    cells.Add(GetCell(from.Col + dx, from.Row + dz));
            }
            return cells;
        }

        public IEnumerable<TCell> GetCellsOnLine(TCell from, TCell to)
        {
            var distance = GetDistance(from, to);

            for (var i = 0; i <= distance; i++)
            {
                var t = 1f / distance * i;

                var x = Lerp(from.x, to.x, t);
                var y = Lerp(from.y, to.y, t);
                var z = Lerp(from.z, to.z, t);

                yield return GetNearestCell(x, y, z);
            }
        }

        private float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
