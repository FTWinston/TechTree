using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Map;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        public Cell(Cell copyFrom)
            : this(copyFrom.ID, copyFrom.Row, copyFrom.Col, copyFrom.Type)
        {
            // Note: we don't copy Building or Units from annother cell (which would be the initial definition, and won't have those).
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public HashSet<Entity> EntitiesThatCanSee { get; } = new HashSet<Entity>();
    }
}
