using GameModels.Instances;
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
