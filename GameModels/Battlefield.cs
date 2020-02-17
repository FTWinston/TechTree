using GameModels.Instances;
using GameModels.Map;
using System.Collections.Generic;

namespace GameModels
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
