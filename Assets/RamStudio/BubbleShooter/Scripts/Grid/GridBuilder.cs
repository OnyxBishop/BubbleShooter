using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts
{
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] [Range(1, 10)] private int _slingshotPositionOffsetY;

        public HexGrid Build(BubbleSpawner spawner, Slingshot slingshot)
        {
            Camera camera = Camera.main;
            var screenHeight = 2f * camera.orthographicSize;
            var screenWidth = camera.aspect * screenHeight;
            
            var center = new Vector2(0, 0);
            var leftWall = new Vector2(center.x - screenWidth / 4, 0);
            var rightWall = new Vector2(center.x + screenWidth / 4, 0);
            var bottomWall = new Vector2(0, center.y - screenHeight / 2);
            var topWall = new Vector2(0,  center.y + screenHeight / 2f);

            var hexRadius = HexExtensions.HexRadius;

            var availableWidth = rightWall.x - leftWall.x - 2 * hexRadius;
            var availableHeight = topWall.y - bottomWall.y - (1.5f * hexRadius + hexRadius);

            var columns = CalculateColumns(availableWidth, hexRadius);
            var rows = CalculateRows(availableHeight, hexRadius);

            var boardSize = new Vector2Int(columns, rows);
            
            var centerColumn = Mathf.FloorToInt(columns / 2f);
            var centerRow = Mathf.FloorToInt(rows / 2f);
            var gridCenterWorld = new OffsetCoordinates(centerColumn, centerRow).ToWorld(new Vector2(0,0));
            Debug.Log($"Total boardSize {columns} || {rows} ||| Width {availableWidth} || Heights {availableHeight}");
            
            var topLeft = new Vector2(
                leftWall.x + 2 * hexRadius,
                topWall.y - (1.5f * hexRadius)
            );

            var bottomRight = new Vector2(
                rightWall.x - 2 * hexRadius,
                bottomWall.y + (1.5f * hexRadius));

            var hexGrid = new HexGrid(spawner, topLeft, boardSize, new GridBounds
            {
                Left = leftWall,
                Right = rightWall,
                Top = topWall,
                Bottom = bottomWall
            });

            Debug.Log($"Bounds positions Right {rightWall.x}|{rightWall.y} Top {topWall.x}{topWall.y}");
            Debug.Log($"Bounds positions Left {leftWall.x}|{leftWall.y} Bottom {bottomWall.x}{bottomWall.y}");
            slingshot.transform.position = new Vector2(0, -_slingshotPositionOffsetY);

            return hexGrid;
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