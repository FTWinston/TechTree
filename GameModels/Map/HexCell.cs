namespace GameModels.Map
{
    public class HexCell
    {
        public HexCell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; }
        public int Col { get; }
    }
}
