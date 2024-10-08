using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Grid
{
    public class LevelBuilder : MonoBehaviour
    {
        public HexGrid Build(BubbleSpawner spawner)
        {
            var screenHeight = GetScreenParams(out var screenWidth);
            screenHeight -= BootstrapInfo.SafeAreaOffsetY;
            var hexGrid = ComputeHexGrid(spawner, screenWidth, screenHeight);

            return hexGrid;
        }

        private HexGrid ComputeHexGrid(BubbleSpawner spawner, float screenWidth, float screenHeight)
        {
            var leftWall = ComputeBounds(screenWidth, screenHeight, out var rightWall, out var bottomWall,
                out var topWall, out var hexRadius, out var columns, out var rows);

            var boardSize = new Vector2Int(columns, rows);

            var topLeft = new Vector2(
                leftWall.x + 2 * hexRadius,
                topWall.y - (1.5f * hexRadius)
            );

            var hexGrid = new HexGrid(spawner, topLeft, boardSize, new GridBounds
            {
                Left = leftWall,
                Right = rightWall,
                Top = topWall,
                Bottom = bottomWall
            });
            return hexGrid;
        }

        private Vector2 ComputeBounds(float screenWidth, float screenHeight, out Vector2 rightWall,
            out Vector2 bottomWall,
            out Vector2 topWall, out float hexRadius, out int columns, out int rows)
        {
            var center = new Vector2(0, 0);
            var leftWall = new Vector2(center.x - screenWidth / 4, 0);
            rightWall = new Vector2(center.x + screenWidth / 4, 0);
            bottomWall = new Vector2(0, center.y - screenHeight / 2);
            topWall = new Vector2(0, center.y + screenHeight / 2f);

            hexRadius = HexExtensions.HexRadius;

            var availableWidth = rightWall.x - leftWall.x - 2 * hexRadius;
            var availableHeight = topWall.y - bottomWall.y - (1.5f * hexRadius + hexRadius);

            columns = CalculateColumns(availableWidth, hexRadius);
            rows = CalculateRows(availableHeight, hexRadius);
            return leftWall;
        }

        private static float GetScreenParams(out float screenWidth)
        {
            var camera = Camera.main;
            var screenHeight = 2f * camera.orthographicSize;
            screenWidth = camera.aspect * screenHeight;
            return screenHeight;
        }

        private int CalculateColumns(float width, float hexRadius)
        {
            var horizontalSpacing = Mathf.Sqrt(3) * hexRadius;

            var columns = Mathf.FloorToInt((width + hexRadius) / horizontalSpacing);
            return Mathf.Max(columns, 1);
        }

        private int CalculateRows(float height, float hexRadius)
        {
            var verticalSpacing = 1.5f * hexRadius;

            var rows = Mathf.FloorToInt((height + (Mathf.Sqrt(3) * hexRadius / 2)) / verticalSpacing);
            rows++;
            return Mathf.Max(rows, 1);
        }
    }
}