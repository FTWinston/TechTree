using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class BattlefieldReadConverter : JsonConverter<Battlefield>
    {
        private class BattlefieldDTO
        {
            public int Width { get; set; }

            public int Height { get; set; }

            public int[]? StartPositions { get; set; }

            public CellDTO?[]? Cells { get; set; }
        }

        private class CellDTO
        {
            public CellType Type { get; set; }

            public Building? Building { get; set; }

            public Unit[]? Units { get; set; }
        }

        public override Battlefield Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<BattlefieldDTO>(ref reader, options);

            if (dto.StartPositions == null || dto.Cells == null)
                throw new JsonException();

            var battlefield = new Battlefield(dto.Width, dto.Height);

            if (dto.Cells.Length != battlefield.Cells.Length)
                throw new JsonException("Inconsistent number of cells in battlefield");

            for (int iCell = 0; iCell < dto.Cells.Length; iCell++)
            {
                var cellDto = dto.Cells[iCell];

                if (cellDto == null)
                    continue;

                int row = battlefield.GetRow(iCell);
                int col = battlefield.GetCol(iCell);
                var cell = new Cell(iCell, row, col, cellDto.Type);

                cell.Building = cellDto.Building;

                if (cellDto.Units != null)
                    cell.Units.AddRange(cellDto.Units);

                battlefield.Cells[iCell] = cell;
            }

            foreach (var index in dto.StartPositions)
                if (index > battlefield.Cells.Length || battlefield.Cells[index] == null)
                    throw new JsonException("Invalid start position");

            battlefield.StartPositions.AddRange(dto.StartPositions);

            return battlefield;
        }

        public override void Write(Utf8JsonWriter writer, Battlefield value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
