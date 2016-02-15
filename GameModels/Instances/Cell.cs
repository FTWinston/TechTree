using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Instances
{
    public class Cell
    {
        [JsonIgnore]
        public Entity Entity { get; set; }
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

        public enum CellType
        {
            OutOfBounds = 0,
            Flat = 1,
            Difficult = 2,
            Unpassable = 3,
            LowBarrier = 4,
            Barrier = 5,
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
                    case CellType.Barrier:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
