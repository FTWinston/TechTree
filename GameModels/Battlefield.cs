using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Map;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels
{
    public class Battlefield : HexMap<Cell>
    {
        public Battlefield(int width, int height)
            : base(width, height)
        {
            StartPositions = new List<int>();
        }

        public Battlefield(Battlefield copyFrom)
            : base(copyFrom.Width, copyFrom.Height)
        {
            StartPositions = new List<int>(copyFrom.StartPositions);

            for (int i = 0; i < copyFrom.Cells.Length; i++)
            {
                var existing = copyFrom.Cells[i];
                if (existing != null)
                    Cells[i] = new Cell(existing);
            }
        }

        public List<int> StartPositions { get; }
    }
}
