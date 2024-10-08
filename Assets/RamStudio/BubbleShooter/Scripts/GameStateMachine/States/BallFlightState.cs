using System.Collections.Generic;
using DG.Tweening;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Grid;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class BallFlightState : IStateWithPayload<Bubble>
    {
        private readonly StateMachine _stateMachine;
        private readonly Slingshot _slingshot;
        private readonly HexGrid _hexGrid;

        private readonly float _shakeDuration = 0.5f;
        private readonly float _shakeDistance = 0.25f;
        private readonly int _shakeVibrato = 10;

        private Bubble _launchedBubble;

        public BallFlightState(StateMachine stateMachine, Slingshot slingshot, HexGrid grid)
        {
            _stateMachine = stateMachine;
            _slingshot = slingshot;
            _hexGrid = grid;
        }

        public void Enter(Bubble bubble)
        {
            _launchedBubble = bubble;
            _slingshot.BallLanded += OnBallLanded;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            _slingshot.BallLanded -= OnBallLanded;
            _launchedBubble = null;
        }

        private void OnBallLanded()
        {
            var ballPosition = (Vector2)_launchedBubble.transform.position;

            if(CheckOutOfBounce(ballPosition))
                return;
            
            var offset = ballPosition.ToOffset(_hexGrid.Origin);
            var cell = _hexGrid.TryGetCellAtPosition(offset);
            
            if(cell is { IsEmpty: false })
                cell.ReplaceBubble(_launchedBubble);
            
            var neighbours = _hexGrid.GetNeighbours(cell, (neighbour) => !neighbour.IsEmpty);

            if (neighbours.Count > 0)
            {
                Shake(neighbours);
                _hexGrid.InsertBubble(cell, _launchedBubble);
                
                var topLeftCoordinates =
                    HexExtensions.GetNeighbourOffset(cell.OffsetCoordinates, NeighboursNames.TopLeft);
                
                var topRightCoordinates =
                    HexExtensions.GetNeighbourOffset(cell.OffsetCoordinates, NeighboursNames.TopRight);
                
                foreach (var neighbour in neighbours)
                    if (neighbour.OffsetCoordinates.Equals(topLeftCoordinates) ||
                        neighbour.OffsetCoordinates.Equals(topRightCoordinates))
                        cell.IncreaseConnectedness();
                
                _stateMachine.ChangeState<CheckColorClusterState, HexCell>(cell);
            }
            else
            {
                _launchedBubble.OnPop();
                _stateMachine.ChangeState<CheckEndConditionState>();
            }
        }

        private void Shake(IReadOnlyCollection<HexCell> neighbours)
        {
            foreach (var neighbour in neighbours)
            {
                var bubbleTransform = neighbour.Bubble?.transform;

                if (bubbleTransform == null) 
                    continue;
                
                Vector2 originalPos = bubbleTransform.position;
                var direction = (originalPos - (Vector2)_launchedBubble.transform.position).normalized;

                neighbour.Bubble.transform.DOShakePosition(_shakeDuration,
                        strength: direction * _shakeDistance,
                        vibrato: _shakeVibrato)
                    .SetEase(Ease.OutSine);
            }
        }

        private bool CheckOutOfBounce(Vector2 ballPosition)
        {
            if (!(ballPosition.y >= _hexGrid.Bounds.Top.y)) 
                return false;
            
            _launchedBubble.OnPop();
            _stateMachine.ChangeState<ReloadSlingshotState>();
            return true;
        }
    }
}