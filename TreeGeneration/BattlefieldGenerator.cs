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
            var mutableCells = allCells
                .Where(cell => cell.Type != CellType.OutOfBounds) // This is a placeholder only
                .ToList();


            for (var i = 0; i < 8; i++)
            {
                RunCellularAutomataStep(battlefield, mutableCells);
            }

            // TODO: remove unconnected open or water areas

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
                    case 1:
                        type = CellType.Unpassable; break;
                    case 2:
                        type = CellType.Water; break;
                    default:
                        type = CellType.Flat; break;
                }

                cell.Type = type;
            }
        }

        private void RunCellularAutomataStep(Battlefield battlefield, List<Cell> mutableCells)
        {
            var results = new Dictionary<Cell, CellType>();

            foreach (var cell in mutableCells) {
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

                CellType newType;

                if (cell.Type == CellType.Unpassable)
                {
                    if (numAdjacentUnpassable < 3)
                        newType = CellType.Flat;
                    else
                        continue;
                }
                else if (cell.Type == CellType.Water)
                {
                    if (numAdjacentWater < 3)
                        newType = CellType.Flat;
                    else
                        continue;
                }
                else if (cell.Type == CellType.Flat)
                {
                    if (numAdjacentUnpassable > 3)
                        newType = CellType.Unpassable;
                    else if (numAdjacentWater > 3)
                        newType = CellType.Water;
                    else
                        continue;
                }
                else
                    continue;

                results[cell] = newType;
            }

            foreach (var kvp in results)
            {
                kvp.Key.Type = kvp.Value;
            }
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
