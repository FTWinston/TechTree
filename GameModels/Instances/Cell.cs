using GameModels.Definitions;
using Newtonsoft.Json;

namespace GameModels.Instances
{
    public class Cell
    {
        [JsonIgnore]
        public Entity Entity { get; internal set; }

        [JsonIgnore]
        public int Col { get; set; }

        [JsonIgnore]
        public int Row { get; set; }

        public CellType Type { get; set; }

        public Cell(int row, int col, CellType type)
        {
            Row = row;
            Col = col;
            Type = type;
        }

        [JsonIgnore]
        public bool IsPassable
        {
            get
            {
                switch(Type)
                {
                    case CellType.Flat:
                    case CellType.Difficult:
                        return true;
                    default:
                        return false;
                }
            }
        }

        [JsonIgnore]
        public bool BlocksVision
        {
            get
            {
                switch (Type)
                {
                    case CellType.OutOfBounds:
                    case CellType.Unpassable:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
