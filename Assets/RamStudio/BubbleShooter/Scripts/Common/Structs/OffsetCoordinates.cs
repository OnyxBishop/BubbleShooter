namespace RamStudio.BubbleShooter.Scripts.Common.Structs
{
    public struct OffsetCoordinates
    {
        public int Column;
        public int Row;

        public OffsetCoordinates(int column, int row)
        {
            Column = column;
            Row = row;
        }
    }
}