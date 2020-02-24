using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Map;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class Cell : HexCell
    {
        public Cell(int id, int row, int col, CellType type)
            : base(row, col)
        {
            ID = id;
            Type = type;
        }

        public int ID { get; }

        public Building? Building { get; set; }

        public List<Unit> Units { get; } = new List<Unit>();

        public CellType Type { get; set; }

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
                    case CellType.Unknown:
                    case CellType.Unpassable:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
