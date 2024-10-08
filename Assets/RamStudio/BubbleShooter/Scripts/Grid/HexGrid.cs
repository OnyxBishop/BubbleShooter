using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.States;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Grid
{
    public class HexGrid
    {
        private readonly int _width, _height;
        private readonly HexCell[,] _cells;
        private readonly BubbleSpawner _spawner;
        private readonly Dictionary<Vector2, HexCell> _cellsPositions;
        private readonly float _fallAnimationDuration = 1.2f;

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

        public event Action<IReadOnlyList<Bubble>> Initialized;
        public event Action<Bubble> Inserted;
        public GridBounds Bounds { get; }
        public Vector2 Origin { get; }

        public void SetBubblesTo(BubbleColors[,] bubbles)
        {
            var columns = bubbles.GetLength(0);
            var rows = bubbles.GetLength(1);
            var firstRow = new List<Bubble>(columns * rows);

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

                    if (row == 0)
                        firstRow.Add(bubble);

                    var cell = _cells[column, row];

                    cell.SetBubble(bubble);
                    InitConnectedWeight(cell, row, column, maxColumns);
                    cell.Disconnected += OnCellDisconnected;
                }
            }

            Initialized?.Invoke(firstRow);
        }

        public void DropAllBubbles(object sender, Action callback)
        {
            if (sender is not GameEndState)
                throw new ArgumentException($"Sender is not valid.");

            var sequence = DOTween.Sequence();

            for (var row = 0; row < _height; row++)
            {
                for (var column = 0; column < _width; column++)
                {
                    var bubble = _cells[column, row].Bubble;

                    if (bubble)
                        sequence.Append(bubble.transform
                                .DOMoveY(Bounds.Bottom.y, _fallAnimationDuration * 0.5f));
                }
            }
            
            sequence.OnComplete(() => callback?.Invoke());
        }

        public void NotifyChangeConnectedness(HexCell cell, NotificationTypes notification,
            params NeighboursNames[] neighbours)
        {
            foreach (var neighbour in neighbours)
            {
                var offset = HexExtensions.GetNeighbourOffset(cell.OffsetCoordinates, neighbour);

                if (!ValidateOffset(offset))
                    continue;

                if (notification == NotificationTypes.Decrease)
                    _cells[offset.Column, offset.Row].DecreaseConnectedness();
                else
                    _cells[offset.Column, offset.Row].IncreaseConnectedness();
            }
        }

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

        private void InitConnectedWeight(HexCell cell, int row, int column, int maxColumns)
        {
            if (row == 0)
            {
                cell.IncreaseConnectedness((byte)ConnectedTypes.Root);
            }
            else if (column == 0)
            {
                if (HexExtensions.IsOdd(row))
                    cell.IncreaseConnectedness((byte)ConnectedTypes.Full);
                else
                    cell.IncreaseConnectedness((byte)ConnectedTypes.Half);
            }
            else if (column == maxColumns - 1)
            {
                if (HexExtensions.IsOdd(row))
                    cell.IncreaseConnectedness((byte)ConnectedTypes.Half);
                else
                    cell.IncreaseConnectedness((byte)ConnectedTypes.Full);
            }
            else
            {
                cell.IncreaseConnectedness((byte)ConnectedTypes.Full);
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

        public void InsertBubble(HexCell cell, Bubble bubble)
        {
            cell.SetBubble(bubble);
            Inserted?.Invoke(bubble);
            cell.Disconnected += OnCellDisconnected;
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
            if (cell == null)
                return new List<HexCell>();

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

            return neighbors;
        }

        #endregion

        private void OnCellDisconnected(HexCell cell)
        {
            cell.Disconnected -= OnCellDisconnected;

            cell.Bubble?.transform.DOMoveY(Bounds.Bottom.y, _fallAnimationDuration)
                .OnComplete(cell.PopBubble);

            NotifyChangeConnectedness(cell, NotificationTypes.Decrease, NeighboursNames.BottomLeft,
                NeighboursNames.BottomRight);
        }

        private bool ValidateOffset(OffsetCoordinates offset)
            => offset.Column >= 0 && offset.Column < _width && offset.Row >= 0 && offset.Row < _height;
    }
}