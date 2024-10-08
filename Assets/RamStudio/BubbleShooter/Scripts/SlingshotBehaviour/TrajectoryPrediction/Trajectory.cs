using System;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.Grid;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SlingshotBehaviour.TrajectoryPrediction
{
    public class Trajectory : MonoBehaviour
    {
        [Header("LineRenderers")]
        [SerializeField] private LineRenderer _mainLine;
        [SerializeField] private LineRenderer _leftSpreadLine;
        [SerializeField] private LineRenderer _rightSpreadLine;
        [SerializeField] private float _spreadAmount;

        [Header("Configuration")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] [Range(20, 100)] private int _maxPredictIterations;
        [SerializeField] [Range(0.05f, 1f)] private float _timeStep = 0.1f;
        [SerializeField] [Range(0.7f, 0.99f)] private float _spreadThreshold = 0.95f;
        [SerializeField] private int _maxForce;
        [SerializeField] private int _maxBounces = 2;
        [SerializeField] private float _dragAreaRadius;

        private Vector2 _slingshotStripsPosition;

        private IInput _input;
        private Vector2 _previousMousePosition;
        private HexGrid _hexGrid;

        private Vector3[] _calculatedPoints;
        private List<Vector2> _trajectoryContactPoints;
        private float _maxTension;
        private float _currentForce;
        private bool _isInDraggingArea;

        public event Action<Vector2> Aiming;
        public event Action<IReadOnlyList<Vector2>, float> HasContacts;

        public Vector2 FirePointPosition => _firePoint.transform.position;

        private void Awake()
        {
            _slingshotStripsPosition = FirePointPosition;
            _maxTension = _dragAreaRadius - 0.25f;

            _calculatedPoints = new Vector3[_maxPredictIterations];
            _trajectoryContactPoints = new List<Vector2>(4);
            _mainLine.positionCount = _maxPredictIterations;
            _leftSpreadLine.positionCount = _maxPredictIterations;
            _rightSpreadLine.positionCount = _maxPredictIterations;
        }

        private void Start()
        {
            _input.BeginDrag += OnBeginDrag;
            _input.Dragging += OnDragging;
            _input.EndDrag += OnEndDrag;

            _calculatedPoints[0] = FirePointPosition;
        }

        private void OnDestroy()
        {
            _input.BeginDrag -= OnBeginDrag;
            _input.Dragging -= OnDragging;
            _input.EndDrag -= OnEndDrag;
        }

        public void Init(IInput input, HexGrid grid)
        {
            _input = input;
            _hexGrid = grid;
        }

        #region EventsHandlers

        private void OnBeginDrag(Vector2 position)
            => _isInDraggingArea = (FirePointPosition - position).sqrMagnitude <= _dragAreaRadius * _dragAreaRadius;

        private void OnDragging(Vector2 mousePosition)
        {
            if (!_isInDraggingArea)
                return;

            if (mousePosition == _previousMousePosition)
                return;

            _previousMousePosition = mousePosition;

            var clickedPosition =
                FirePointPosition + Vector2.ClampMagnitude(mousePosition - FirePointPosition, _dragAreaRadius);

            _slingshotStripsPosition = clickedPosition;

            var pullVector = FirePointPosition - _slingshotStripsPosition;
            _currentForce = pullVector.magnitude / _maxTension;

            CalculateMovement(pullVector, !(_currentForce < _spreadThreshold));

            Draw(_calculatedPoints);
            Aiming?.Invoke(_slingshotStripsPosition);
        }

        private void OnEndDrag()
        {
            if (!_isInDraggingArea)
                return;
            
            DisableView();
            HasContacts?.Invoke(_trajectoryContactPoints, _currentForce);
        }

        #endregion

        #region TrajectoryMovement

        private void CalculateMovement(Vector2 initialVelocity, bool isHighForce)
        {
            var velocity = initialVelocity.normalized * (_currentForce * _maxForce);

            var currentPosition = FirePointPosition;
            var currentVelocity = velocity;

            var bounceCount = -1;
            var dynamicTimeStep = _timeStep * _currentForce / currentVelocity.magnitude;
            _trajectoryContactPoints.Clear();
            _trajectoryContactPoints.Add(FirePointPosition);

            for (var iteration = 1; iteration < _maxPredictIterations; iteration++)
            {
                var nextPosition = currentPosition + currentVelocity * dynamicTimeStep;

                nextPosition = HandleWallCollisions(nextPosition, ref bounceCount, ref currentVelocity,
                    out var hasWallCollision);

                if (hasWallCollision)
                {
                    _calculatedPoints[iteration] = nextPosition;
                    _trajectoryContactPoints.Add(nextPosition);

                    if (bounceCount >= _maxBounces)
                    {
                        var lastValidPosition = _calculatedPoints[iteration - 1];
                        CollapsePointsAtPosition(iteration, lastValidPosition);
                        break;
                    }

                    continue;
                }

                if (TryHandleBubbleCollision(nextPosition, isHighForce, iteration))
                {
                    _trajectoryContactPoints.Add(nextPosition);
                    break;
                }

                _calculatedPoints[iteration] = nextPosition;
                currentPosition = nextPosition;

                // currentVelocity += Physics2D.gravity * dynamicTimeStep;
            }
            
            _trajectoryContactPoints.Add(_calculatedPoints[^1]);
        }

        private Vector2 SetBubblePosition(Vector2 hitPosition, HexCell hittedCell)
        {
            if (hitPosition.x <= hittedCell.WorldPosition.x && hitPosition.y > hittedCell.WorldPosition.y)
            {
                var leftOffset = HexExtensions.GetNeighbourOffset(hittedCell.OffsetCoordinates, NeighboursNames.Left);

                if (_hexGrid.TryGetEmptyNeighbor(leftOffset, out var cell))
                    return cell.WorldPosition;
            }

            if (hitPosition.x > hittedCell.WorldPosition.x && hitPosition.y > hittedCell.WorldPosition.y)
            {
                var rightOffset = HexExtensions.GetNeighbourOffset(hittedCell.OffsetCoordinates, NeighboursNames.Right);

                if (_hexGrid.TryGetEmptyNeighbor(rightOffset, out var cell))
                    return cell.WorldPosition;
            }

            if (hitPosition.x <= hittedCell.WorldPosition.x && hitPosition.y <= hittedCell.WorldPosition.y)
            {
                var bottomLeftOffset =
                    HexExtensions.GetNeighbourOffset(hittedCell.OffsetCoordinates, NeighboursNames.BottomLeft);

                if (_hexGrid.TryGetEmptyNeighbor(bottomLeftOffset, out var cell))
                    return cell.WorldPosition;
            }

            if (hitPosition.x > hittedCell.WorldPosition.x && hitPosition.y <= hittedCell.WorldPosition.y)
            {
                var bottomRightOffset =
                    HexExtensions.GetNeighbourOffset(hittedCell.OffsetCoordinates, NeighboursNames.BottomRight);

                if (_hexGrid.TryGetEmptyNeighbor(bottomRightOffset, out var cell))
                    return cell.WorldPosition;
            }

            return Vector2.zero;
        }

        private bool TestBubbleHit(Vector2 position, out HexCell cell)
        {
            cell = _hexGrid.TryGetCellAtPosition(position);
            return cell is { IsEmpty: false };
        }

        private void CollapsePointsAtPosition(int startIteration, Vector2 stopPosition)
        {
            for (var j = startIteration; j < _maxPredictIterations; j++)
                _calculatedPoints[j] = stopPosition;
        }

        private Vector2 HandleWallCollisions(Vector2 nextPosition, ref int bounceCount,
            ref Vector2 velocity,
            out bool isCollision)
        {
            isCollision = false;
            var minX = _hexGrid.Bounds.Left.x + HexExtensions.HexRadius;
            var maxX = _hexGrid.Bounds.Right.x - HexExtensions.HexRadius;
            var minY = _hexGrid.Bounds.Bottom.y;
            var maxY = _hexGrid.Bounds.Top.y - HexExtensions.HexRadius;

            if (nextPosition.x >= maxX || nextPosition.x <= minX)
            {
                velocity.x = -velocity.x;
                nextPosition.x = nextPosition.x >= maxX ? maxX : minX;
                bounceCount++;
                isCollision = true;
            }

            if (nextPosition.y >= maxY || nextPosition.y <= minY)
            {
                bounceCount += 2;
                isCollision = true;
            }

            return nextPosition;
        }

        private bool TryHandleBubbleCollision(Vector2 nextPosition, bool isHighForce, int iteration)
        {
            if (!TestBubbleHit(nextPosition, out var cell))
                return false;

            var newBubblePosition = isHighForce
                ? cell.WorldPosition
                : SetBubblePosition(nextPosition, cell);

            if (!isHighForce && newBubblePosition == Vector2.zero)
                newBubblePosition = nextPosition;

            CollapsePointsAtPosition(iteration - 1, newBubblePosition);
            return true;
        }

        #endregion

        #region DrawTrajectory

        private void Draw(Vector3[] points)
        {
            if (_currentForce < _spreadThreshold)
            {
                _mainLine.SetPositions(points);
                DisableSpread();
            }
            else
            {
                _leftSpreadLine.SetPosition(0, FirePointPosition);
                _rightSpreadLine.SetPosition(0, FirePointPosition);

                for (var i = 1; i < points.Length; i++)
                {
                    var stepSpread = i * _spreadAmount;

                    var leftPoint = points[i];
                    leftPoint.x -= stepSpread;
                    _leftSpreadLine.SetPosition(i, leftPoint);

                    var rightPoint = points[i];
                    rightPoint.x += stepSpread;
                    _rightSpreadLine.SetPosition(i, rightPoint);
                }

                EnableSpread();
            }
        }

        private void EnableSpread()
        {
            _mainLine.enabled = false;
            _leftSpreadLine.enabled = true;
            _rightSpreadLine.enabled = true;
        }

        private void DisableSpread()
        {
            _mainLine.enabled = true;
            _leftSpreadLine.enabled = false;
            _rightSpreadLine.enabled = false;
        }

        private void DisableView()
        {
            _mainLine.enabled = false;
            _leftSpreadLine.enabled = false;
            _rightSpreadLine.enabled = false;
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_calculatedPoints != null && _calculatedPoints.Length > 1)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _calculatedPoints.Length - 1; i++)
                {
                    Vector3 startPoint = _calculatedPoints[i];
                    Vector3 endPoint = _calculatedPoints[i + 1];
                    Gizmos.DrawLine(startPoint, endPoint);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(endPoint, 0.1f);
                }

                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(_calculatedPoints[^1], 0.2f);
            }
        }
#endif
    }
}