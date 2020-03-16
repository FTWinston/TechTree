using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Map
{
    public class HexCell
    {
        public HexCell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        [JsonIgnore]
        public int Row { get; }
        
        [JsonIgnore]
        public int Col { get; }
    }
}
