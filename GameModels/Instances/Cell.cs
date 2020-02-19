﻿using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Map;
using Newtonsoft.Json;

namespace ObjectiveStrategy.GameModels.Instances
{
    public class Cell : HexCell
    {
        public Cell (int row, int col, CellType type)
            : base(row, col)
        {
            Type = type;
        }

        [JsonIgnore]
        public Entity? Entity { get; set; }

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
