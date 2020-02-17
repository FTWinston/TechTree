using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameGenerator.BattlefieldGeneration
{
    public class BattlefieldGenerator
    {
        public BattlefieldGenerator(int complexity, int seed)
        {
            Complexity = complexity;
            Seed = seed;
        }

        private Random Random { get; set; }

        public int Complexity { get; }

        public int Seed { get; }

        public Battlefield Generate()
        {
            Random = new Random(Seed);

            DetermineSize(out int width, out int height);
            var battlefield = new Battlefield(width, height);

            CreateCells(battlefield);

            List<Tuple<Cell, Cell>> mirroredCells = MirrorCells(battlefield);

            List<Tuple<Cell, Cell>> startAreaCells = PickStartCells(battlefield, mirroredCells);

            var mutableCells = mirroredCells
                .Where(cells => !startAreaCells.Contains(cells))
                .ToList();

            PlaceUnpassableAndWater(battlefield, mutableCells);

            PlaceDifficult(battlefield, mutableCells);

            return battlefield;
        }

        private void DetermineSize(out int width, out int height)
        {
            const int minComplexitySize = 15; // Complexity 1 size 15 x 15
            const int maxComplexitySize = 51; // Complexity 100 size 51 x 51

            float complexityFraction = (Complexity - 1f) / (GameGenerator.MaxComplexity - 1f);

            // TODO: variation within width and height would be nice to have here

            float medianSize = minComplexitySize + (maxComplexitySize - minComplexitySize) * complexityFraction;

            int size = (int)Math.Round(medianSize);

            // Symmetry fails when size is even.
            if (size % 2 == 0)
            {
                if (medianSize > size)
                    size++;
                else
                    size--;
            }

            width = size;
            height = size;
        }

        private static void ApplyToAll(Tuple<Cell, Cell> cells, Action<Cell> action)
        {
            action(cells.Item1);
            action(cells.Item2);
        }

        private void CreateCells(Battlefield battlefield)
        {
            foreach (var position in GetValidCellPositions(battlefield.Width, battlefield.Height))
            {
                var cell = new Cell(position.Row, position.Col, CellType.Flat);
                battlefield.Cells[position.GetCellIndex(battlefield.Width)] = cell;
            }
        }

        private List<Tuple<Cell, Cell>> MirrorCells(Battlefield battlefield)
        {
            // TODO: allow multiple axes of symmetry, and mirroring vs rotation

            // For now, just do a rotational mirror vertically.
            var allCells = battlefield.Cells
                .Where(cell => cell != null)
                .ToArray();

            int halfLength = (int)Math.Ceiling(allCells.Length / 2.0);

            // If there's a "center" cell, it will be in a tuple with itself, but that's fine.            
            return allCells.Take(halfLength)
                .Select((cell, index) => new Tuple<Cell, Cell>(cell, allCells[allCells.Length - 1 - index]))
                .ToList();
        }

        private List<Tuple<Cell, Cell>> PickStartCells(Battlefield battlefield, List<Tuple<Cell, Cell>> mirroredCells)
        {
            int minDist = (int)(battlefield.Width * 0.25 + battlefield.Height * 0.25);
            Tuple<Cell, Cell> startCells;

            do
            {
                startCells = mirroredCells.PickRandom(Random);
            } while (battlefield.GetDistance(startCells.Item1, startCells.Item2) < minDist);

            battlefield.StartPositions.Add(Array.IndexOf(battlefield.Cells, startCells.Item1));
            battlefield.StartPositions.Add(Array.IndexOf(battlefield.Cells, startCells.Item2));

            var startAreaCells = battlefield.GetCellsWithinDistance(startCells.Item1, 2);

            var startAreaCellTuples = mirroredCells
                .Where(cells => startAreaCells.Contains(cells.Item1))
                .ToList();

            foreach (var cells in startAreaCellTuples)
            {
                ApplyToAll(cells, cell => cell.Type = CellType.Flat);
            }

            return startAreaCellTuples;
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

        private void RandomizeCells(IEnumerable<Tuple<Cell, Cell>> allCells)
        {
            // randomly make each cell either flat, water, or unpassable
            foreach (var cells in allCells)
            {
                CellType type;
                switch (Random.Next(3))
                {
                    case 0:
                        type = CellType.Unpassable; break;
                    case 1:
                        type = CellType.Water; break;
                    default:
                        type = CellType.Flat; break;
                }

                ApplyToAll(cells, cell => cell.Type = type);
            }
        }

        private void PlaceUnpassableAndWater(Battlefield battlefield, List<Tuple<Cell, Cell>> mutableCells)
        {
            RandomizeCells(mutableCells);

            for (var i = 0; i < 8; i++)
            {
                RunCellularAutomataStep(battlefield, mutableCells, ResolveNewWaterAndUnpassableType);
            }

            // TODO: fill unconnected land or water areas with unpassable
        }

        private void PlaceDifficult(Battlefield battlefield, List<Tuple<Cell, Cell>> mutableCells)
        {
            var flatCells = mutableCells
                .Where(cell => cell.Item1.Type == CellType.Flat)
                .ToList();

            MakeDifficult(flatCells);

            for (var i = 0; i < 8; i++)
            {
                RunCellularAutomataStep(battlefield, mutableCells, ResolveNewDifficultType);
            }
        }

        private void RunCellularAutomataStep(Battlefield battlefield, IEnumerable<Tuple<Cell, Cell>> mutableCells, Func<Cell, Battlefield, CellType?> rules)
        {
            var results = new Dictionary<Tuple<Cell, Cell>, CellType>();

            foreach (var cells in mutableCells) {
                var newType = rules(cells.Item1, battlefield);

                if (newType.HasValue)
                    results[cells] = newType.Value;
            }

            foreach (var kvp in results)
            {
                ApplyToAll(kvp.Key, cell => cell.Type = kvp.Value);
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

        private void MakeDifficult(IEnumerable<Tuple<Cell, Cell>> flatCells)
        {
            foreach (var cells in flatCells)
            {
                if (Random.NextDouble() < 0.3)
                    ApplyToAll(cells, cell => cell.Type = CellType.Difficult);
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
