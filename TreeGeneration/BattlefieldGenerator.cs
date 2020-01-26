using GameModels;
using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeGeneration
{
    public class BattlefieldGenerator
    {
        public int Width { get; set; } = 37;
        public int Height { get; set; } = 37;

        public Battlefield Generate()
        {
            Random random = new Random();
            var battlefield = new Battlefield(Width, Height);

            List<Cell> allCells = CreateCells(battlefield);

            RandomizeCells(random, allCells);

            // TODO: place start positions and resources, ensure paths between them
            // TODO: remove these from the list of mutable cells
            var mutableCells = new List<Cell>(allCells);

            for (var i = 0; i < 8; i++)
            {
                RunCellularAutomataStep(battlefield, mutableCells, ResolveNewWaterAndUnpassableType);
            }

            // TODO: fill unconnected land or water areas with unpassable

            var flatCells = mutableCells
                .Where(cell => cell.Type == CellType.Flat)
                .ToList();

            MakeDifficult(random, flatCells);

            for (var i = 0; i < 8; i++)
            {
                RunCellularAutomataStep(battlefield, mutableCells, ResolveNewDifficultType);
            }

            return battlefield;
        }

        private List<Cell> CreateCells(Battlefield map)
        {
            var allCells = new List<Cell>();

            foreach (var position in GetValidCellPositions(Width, Height))
            {
                var cell = new Cell(position.Row, position.Col, CellType.Flat);
                allCells.Add(cell);
                map.Cells[position.GetCellIndex(Width)] = cell;
            }

            return allCells;
        }

        private IEnumerable<MapPosition> GetValidCellPositions(int width, int height)
        {
            var halfSize = Math.Max(width, height) / 2.0f;
            var halfLower = (int)halfSize;
            var halfUpper = (int)(halfSize + 0.5f);

            for (var col = 0; col < width; col++)
                for (var row = 0; row < height; row++)
                    if (col + row >= halfLower && col + row < width + height - halfUpper)
                        yield return new MapPosition { Col = col, Row = row };
        }

        private void RandomizeCells(Random random, List<Cell> allCells)
        {
            // randomly make each cell either flat, water, or unpassable
            foreach (var cell in allCells)
            {
                CellType type;
                switch (random.Next(3))
                {
                    case 0:
                        type = CellType.Unpassable; break;
                    case 1:
                        type = CellType.Water; break;
                    default:
                        type = CellType.Flat; break;
                }

                cell.Type = type;
            }
        }

        private void RunCellularAutomataStep(Battlefield battlefield, List<Cell> mutableCells, Func<Cell, Battlefield, CellType?> rules)
        {
            var results = new Dictionary<Cell, CellType>();

            foreach (var cell in mutableCells) {
                var newType = rules(cell, battlefield);
                if (newType.HasValue)
                    results[cell] = newType.Value;
            }

            foreach (var kvp in results)
            {
                kvp.Key.Type = kvp.Value;
            }
        }

        private CellType? ResolveNewWaterAndUnpassableType(Cell cell, Battlefield battlefield)
        {
            // Water and unpassable becomes flat if < 3 around it have the same type.
            // Flat becomes unpassable if > 3 around it are unpassable,
            // and becomes water if > 3 around it are water.
            var neighbours = battlefield.GetCellsWithinDistance(cell, 1);

            var numAdjacentUnpassable = neighbours
                .Where(c => c?.Type == CellType.Unpassable)
                .Count();

            var numAdjacentWater = neighbours
                .Where(c => c?.Type == CellType.Water)
                .Count();

            if (cell.Type == CellType.Unpassable)
            {
                if (numAdjacentUnpassable < 3)
                    return CellType.Flat;
            }
            else if (cell.Type == CellType.Water)
            {
                if (numAdjacentWater < 3)
                    return CellType.Flat;
            }
            else if (cell.Type == CellType.Flat)
            {
                if (numAdjacentUnpassable > 3)
                    return CellType.Unpassable;
                else if (numAdjacentWater > 3)
                    return CellType.Water;
            }

            return null;
        }

        private void MakeDifficult(Random random, List<Cell> flatCells)
        {
            foreach (var cell in flatCells)
            {
                if (random.NextDouble() < 0.3)
                    cell.Type = CellType.Difficult;
            }
        }


        private CellType? ResolveNewDifficultType(Cell cell, Battlefield battlefield)
        {
            var numAdjacentFlat = battlefield
                .GetCellsWithinDistance(cell, 1)
                .Where(c => c == null || c.Type == CellType.Flat)
                .Count();

            // Flat becomes difficult if < 3 flat around it.
            // Difficult becomes flat if > 3 flat around it.

            if (cell.Type == CellType.Flat)
            {
                if (numAdjacentFlat < 3)
                    return CellType.Difficult;
            }
            else if (cell.Type == CellType.Difficult)
            {
                if (numAdjacentFlat > 3)
                    return CellType.Flat;
            }

            return null;
        }

        class MapPosition
        {
            public int Row { get; set; }
            public int Col { get; set; }

            public int GetCellIndex(int mapWidth)
            {
                return Col + Row * mapWidth;
            }
        }
    }
}
