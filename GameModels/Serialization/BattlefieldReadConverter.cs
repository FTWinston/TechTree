using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class BattlefieldReadConverter : PropertyDictionaryConverter<Battlefield>
    {
        private class CellDTO
        {
            public CellType Type { get; set; }

            public Building? Building { get; set; }

            public Unit[]? Units { get; set; }
        }

        public override Battlefield Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            int? width = 0;
            int? height = 0;
            int[]? startPositions = null;
            CellDTO?[]? cells = null;

            ReadProperties(ref reader, new Dictionary<string, FieldReader>
            {
                {
                    nameof(Battlefield.Width), (ref Utf8JsonReader r) => {
                        if (r.TokenType != JsonTokenType.Number)
                            throw new JsonException();
                        width = r.GetInt32();
                    }
                },
                {
                    nameof(Battlefield.Height), (ref Utf8JsonReader r) =>
                    {
                        if (r.TokenType != JsonTokenType.Number)
                            throw new JsonException();
                        height = r.GetInt32();
                    }
                },
                {
                    nameof(Battlefield.StartPositions), (ref Utf8JsonReader r) =>
                    {
                        startPositions = JsonSerializer.Deserialize<int[]>(ref r, options);
                    }
                },
                {
                    nameof(Battlefield.Cells), (ref Utf8JsonReader r) =>
                    {
                        cells = JsonSerializer.Deserialize<CellDTO?[]>(ref r);
                    }
                },
            });

            if (!width.HasValue || !height.HasValue || startPositions == null || cells == null)
                throw new JsonException();

            var battlefield = new Battlefield
            (
                width.Value,
                height.Value
            );

            if (cells.Length != battlefield.Cells.Length)
                throw new JsonException("Inconsistent number of cells in battlefield");

            for (int iCell = 0; iCell < cells.Length; iCell++)
            {
                var dto = cells[iCell];

                if (dto == null)
                    continue;

                int row = battlefield.GetRow(iCell);
                int col = battlefield.GetCol(iCell);
                var cell = new Cell(iCell, row, col, dto.Type);

                cell.Building = dto.Building;

                if (dto.Units != null)
                    cell.Units.AddRange(dto.Units);

                battlefield.Cells[iCell] = cell;
            }

            foreach (var index in startPositions)
                if (index > battlefield.Cells.Length || battlefield.Cells[index] == null)
                    throw new JsonException("Invalid start position");

            battlefield.StartPositions.AddRange(startPositions);

            return battlefield;
        }
    }
}
