using System;
using System.Collections.Generic;
using System.Linq;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts
{
    public class HexGrid
    {
        private readonly int _width, _height;
        private readonly HexCell[,] _cells;
        private readonly BubbleSpawner _spawner;

        //odd-r
        public HexGrid(BubbleSpawner spawner, Vector2 origin, Vector2Int size, GridBounds bounds)
        {
            _spawner = spawner;
            Origin = origin;
            _width = size.x;
            _height = size.y;
            Bounds = bounds;

            _cells = new HexCell[_width, _height];
            Build(Origin);
        }

        public GridBounds Bounds { get; }
        public Vector2 Origin { get; }

        public void SetBubbles(BubbleColors[,] bubbles)
        {
            var columns = bubbles.GetLength(0);
            var rows = bubbles.GetLength(1);
        
            var maxColumns = Mathf.Min(_width, columns);
            var maxRows = Mathf.Min(_height, rows);
        
            for (var row = 0; row < maxRows; row++)
            {
                for (var column = 0; column < maxColumns; column++)
                {
                    var bubbleType = bubbles[column, row];
        
                    if (bubbleType == BubbleColors.None)
                        continue;
        
                    var bubble = _spawner.Spawn(bubbleType);
                    bubble.gameObject.layer = 7;
                    bubble.name = $"{column} | {row}";
                    bubble.TextMe(column,row);
        
                    _cells[column, row].SetBubble(bubble);
                    _cells[column,row].MarkAsConnected();
                }
            }
        }

        private void Build(Vector2 origin)
        {
            for (var row = 0; row < _height; row++)
            {
                for (var column = 0; column < _width; column++)
                {
                    var coordinates = new OffsetCoordinates(column, row);
                    var hexPosition = coordinates.ToWorld(origin);
                    var cell = new HexCell(coordinates, hexPosition);
                    _cells[column, row] = cell;
                }
            }
        }

        #region CellsOperations

        public bool CheckIsFloating(HexCell cell, out IReadOnlyCollection<HexCell> neighbours)
        {
            if (ValidateOffset(new OffsetCoordinates(cell.OffsetCoordinates.Column, cell.OffsetCoordinates.Row)))
            {
                neighbours = GetNeighbours(cell, (bubbleCell) => !bubbleCell.IsEmpty);
                return false;
            }

            neighbours = null;
            return true;
        }
        
        public bool TryGetEmptyNeighbor(OffsetCoordinates offset, out HexCell cell)
        {
            if (ValidateOffset(offset))
            {
                cell = _cells[offset.Column, offset.Row];
                return cell.IsEmpty;
            }

            cell = null;
            return false;
        }

        public HexCell TryGetCellAtPosition(Vector2 position)
        {
            var offset = position.ToOffset(Origin);

            return ValidateOffset(offset)
                ? _cells[offset.Column, offset.Row]
                : null;
        }

        public HexCell TryGetCellAtPosition(OffsetCoordinates offset)
        {
            return ValidateOffset(offset)
                ? _cells[offset.Column, offset.Row]
                : null;
        }

        public IReadOnlyList<HexCell> GetNeighbours(HexCell cell, Func<HexCell, bool> filter = null)
        {
            var column = cell.OffsetCoordinates.Column;
            var row = cell.OffsetCoordinates.Row;

            var currentOffsets = row % 2 == 1
                ? HexExtensions.OddRowsOffsets
                : HexExtensions.EvenRowsOffsets;

            var neighbors = new List<HexCell>();

            foreach (var offset in currentOffsets)
            {
                var neighbourCol = column + offset.Column;
                var neighbourRow = row + offset.Row;

                if (!ValidateOffset(new OffsetCoordinates(neighbourCol, neighbourRow)))
                    continue;

                var neighbor = _cells[neighbourCol, neighbourRow];
                neighbors.Add(neighbor);
            }

            if (filter != null)
                neighbors = neighbors.Where(filter).ToList();

            return neighbors.ToList();
        }

        #endregion

        #region DFS_ClusterSearch

        public IReadOnlyList<HexCell> FindColorCluster(HexCell startCell)
        {
            var cluster = new List<HexCell>();
            var visited = new HashSet<HexCell>();
            var targetColor = startCell.Bubble?.Color;

            var stack = new Stack<HexCell>();
            stack.Push(startCell);

            while (stack.Count > 0)
            {
                var currentCell = stack.Pop();

                if (!visited.Add(currentCell))
                    continue;

                if (!startCell.Bubble)
                    continue;

                if (currentCell.IsEmpty || currentCell.Bubble?.Color != targetColor)
                    continue;

                cluster.Add(currentCell);

                foreach (var neighbor in GetNeighbours(currentCell,
                             (cell) => cell.Bubble && cell.Bubble.Color == targetColor))
                    if (!visited.Contains(neighbor))
                        stack.Push(neighbor);
            }

            return cluster;
        }

        public IReadOnlyCollection<HexCell> FindFloatingCluster()
        {
            var floatingCluster = new List<HexCell>();
            var visited = new HashSet<HexCell>();
            
            var notFloatingCells = MarkConnectedToRoot(visited);
            
            for (var row = 0; row < _height; row++)
            {
                for (var column = 0; column < _width; column++)
                {
                    var cell = _cells[column, row];

                    if (visited.Contains(cell) || cell.IsEmpty)
                        continue;

                    if (notFloatingCells.Contains(cell)) 
                        continue;
                    
                    DepthFirstSearch(cell, floatingCluster, visited);
                }
            }

            return floatingCluster;
        }

        private HashSet<HexCell> MarkConnectedToRoot(HashSet<HexCell> visited)
        {
            var connectedToTop = new HashSet<HexCell>();
            var stack = new Stack<HexCell>();

            for (var column = 0; column < _width; column++)
            {
                var cell = _cells[column, 0];

                if (!cell.Bubble)
                    continue;

                stack.Push(cell);
                connectedToTop.Add(cell);
                visited.Add(cell);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (var neighbor in GetNeighbours(current, (cell) => cell.Bubble && connectedToTop.Add(cell)))
                {
                    stack.Push(neighbor);
                    visited.Add(neighbor);
                }
            }

            return connectedToTop;
        }

        private void DepthFirstSearch(HexCell startCell, List<HexCell> cluster, HashSet<HexCell> visited)
        {
            var stack = new Stack<HexCell>();
            stack.Push(startCell);
            visited.Add(startCell);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                cluster.Add(current);

                foreach (var neighbor in GetNeighbours(current))
                {
                    if (neighbor.Bubble && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        #endregion
        
        private bool ValidateOffset(OffsetCoordinates offset)
            => offset.Column >= 0 && offset.Column < _width && offset.Row >= 0 && offset.Row < _height;
    }
}