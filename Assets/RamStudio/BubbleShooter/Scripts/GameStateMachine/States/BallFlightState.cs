using System.Collections.Generic;
using DG.Tweening;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
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
            var offset = ballPosition.ToOffset(_hexGrid.Origin);
            var cell = _hexGrid.TryGetCellAtPosition(offset);
            var connectedNeighbours = _hexGrid.GetNeighbours(cell, (neighbour) => neighbour.IsConnected);
            
            foreach (var connectedNeighbour in connectedNeighbours)
            {
                Debug.Log($"{connectedNeighbour.OffsetCoordinates.Column}|{connectedNeighbour.OffsetCoordinates.Row}");
            }
            
            if (cell != null && connectedNeighbours.Count > 0)
            {
                Debug.Log($"Должен лопнуть");
                cell.SetBubble(_launchedBubble);
                cell.MarkAsConnected();
                Shake(connectedNeighbours);
                _stateMachine.ChangeState<CheckColorClusterState, HexCell>(cell);
            }
            else
            {
                Debug.Log($"Попал не туда");
                _launchedBubble.OnPop();
                _stateMachine.ChangeState<ReloadSlingshotState>();
            }
        }

        private void Shake(IReadOnlyCollection<HexCell> neighbours)
        {
            foreach (var neighbour in neighbours)
            {
                var bubbleTransform = neighbour.Bubble.transform;
                Vector2 originalPos = bubbleTransform.position;
                var direction = (originalPos - (Vector2)_launchedBubble.transform.position).normalized;
                
                neighbour.Bubble.transform.DOShakePosition(_shakeDuration, 
                        strength: direction * _shakeDistance, 
                        vibrato: _shakeVibrato)
                    .SetEase(Ease.OutSine);
            }
        }
    }
}