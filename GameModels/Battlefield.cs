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
            
        }

        public List<int> StartPositions { get; } = new List<int>();
    }
}
