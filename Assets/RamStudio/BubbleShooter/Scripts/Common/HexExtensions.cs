using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Common
{
    public static class HexExtensions
    {
        public static readonly OffsetCoordinates[] EvenRowsOffsets =
        {
            new(-1, -1), // Top Left
            new(0, -1), // Top right
            new(-1, 0), // Left
            new(+1, 0), // Right
            new(-1, +1), // Bottom Left
            new(0, +1), // Bottom right
        };

        public static readonly OffsetCoordinates[] OddRowsOffsets =
        {
            new(0, -1), // Top Left
            new(+1, -1), // Top right
            new(-1, 0), // Left
            new(+1, 0), // Right
            new(0, +1), // Bottom Left
            new(+1, +1), // Bottom right
        };

        public const float HexRadius = 0.5f;

        public static bool IsOdd(int row) 
            => row % 2 == 1;
        
        public static OffsetCoordinates GetNeighbourOffset(OffsetCoordinates coordinates,
            NeighboursNames neighboursName)
        {
            var offsets = IsOdd(coordinates.Row)
                ? OddRowsOffsets
                : EvenRowsOffsets;

            var needOffset = offsets[(int)neighboursName];

            return new OffsetCoordinates(coordinates.Column + needOffset.Column, coordinates.Row + needOffset.Row);
        }

        public static Vector2 ToWorld(this OffsetCoordinates coordinates, Vector2 origin)
        {
            var horizontalOffset = Mathf.Sqrt(3) * HexRadius;
            var verticalOffset = 1.5f * HexRadius;

            var xOffset = IsOdd(coordinates.Row)
                ? horizontalOffset / 2
                : 0;

            var x = coordinates.Column * horizontalOffset + xOffset;
            var y = -coordinates.Row * verticalOffset;

            return new Vector2(x, y) + origin;
        }

        public static OffsetCoordinates ToOffset(this Vector2 position, Vector2 origin)
        {
            var horizontalOffset = Mathf.Sqrt(3) * HexRadius;
            var verticalOffset = 1.5f * HexRadius;
            var localPos = position - origin;

            var roughRow = Mathf.RoundToInt(-localPos.y / verticalOffset);
            
            var xOffset = IsOdd(roughRow)
                ? horizontalOffset / 2
                : 0;

            var roughColumn = Mathf.RoundToInt((localPos.x - xOffset) / horizontalOffset);

            var closestOffset = new OffsetCoordinates(roughColumn, roughRow);
            
            return closestOffset;
        }
    }
}