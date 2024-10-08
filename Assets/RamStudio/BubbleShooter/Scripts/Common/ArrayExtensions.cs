namespace RamStudio.BubbleShooter.Scripts.Common
{
    public static class ArrayExtensions
    {
        public static T[,] To2dArray<T>(this T[] source, int columns, int rows)
        {
            var to2DArray = new T[columns, rows];
            
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    var index = row * columns + column;
                    to2DArray[column, row] = source[index];
                }
            }

            return to2DArray;
        }
    }
}