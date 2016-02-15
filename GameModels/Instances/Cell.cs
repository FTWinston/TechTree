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

        public enum CellType
        {
            Passable,
            Blocked,
        }
    }
}
