using System.Collections;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Grid;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using RamStudio.BubbleShooter.Scripts.SO;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class CheckColorClusterState : IStateWithPayload<HexCell>
    {
        private readonly StateMachine _stateMachine;
        private readonly HexGrid _grid;
        private readonly Slingshot _slingshot;
        private readonly ScoreStorage _scoreStorage;
        private readonly WaitForSeconds _wait = new(0.1f);
        private readonly int _maxPoints;
        private readonly int _minPoints;

        public CheckColorClusterState(StateMachine stateMachine, HexGrid grid, Slingshot slingshot,
            ScoreStorage scoreStorage, LevelConfiguration configuration)
        {
            _stateMachine = stateMachine;
            _grid = grid;
            _slingshot = slingshot;
            _scoreStorage = scoreStorage;
            _maxPoints = configuration.MaxPointsByBubble;
            _minPoints = configuration.MinPointsByBubble;
        }

        public void Enter(HexCell startCell)
        {
            var neighbours = _grid.FindColorCluster(startCell);

            if (neighbours.Count > 2)
                _slingshot.StartCoroutine(PopBubbles(neighbours));

            _stateMachine.ChangeState<CheckEndConditionState>();
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        private IEnumerator PopBubbles(IReadOnlyList<HexCell> cells)
        {
            foreach (var cell in cells)
            {
                cell.PopBubble();
                _scoreStorage.Add(Random.Range(_minPoints, _maxPoints));
                _grid.NotifyChangeConnectedness(cell, NotificationTypes.Decrease, NeighboursNames.BottomLeft,
                    NeighboursNames.BottomRight);
                yield return _wait;
            }
        }
    }
}