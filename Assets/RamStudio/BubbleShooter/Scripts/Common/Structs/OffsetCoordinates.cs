using System;

namespace RamStudio.BubbleShooter.Scripts.Common.Structs
{
    public readonly struct OffsetCoordinates : IEquatable<OffsetCoordinates>
    {
        public readonly int Column;
        public readonly int Row;

        public OffsetCoordinates(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public override bool Equals(object obj)
        {
            if (obj is OffsetCoordinates other)
            {
                return Equals(other);
            }
            
            return false;
        }
        
        public bool Equals(OffsetCoordinates other)
        {
            return Column == other.Column && Row == other.Row;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Column.GetHashCode();
                hash = hash * 23 + Row.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(OffsetCoordinates left, OffsetCoordinates right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OffsetCoordinates left, OffsetCoordinates right)
        {
            return !(left == right);
        }
    }
}