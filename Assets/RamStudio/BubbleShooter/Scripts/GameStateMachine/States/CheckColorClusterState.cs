using System.Collections;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class CheckColorClusterState : IStateWithPayload<HexCell>
    {
        private readonly StateMachine _stateMachine;
        private readonly HexGrid _grid;
        private readonly Slingshot _slingshot;
        private readonly WaitForSeconds _wait = new (0.1f);

        public CheckColorClusterState(StateMachine stateMachine, HexGrid grid, Slingshot slingshot)
        {
            _stateMachine = stateMachine;
            _grid = grid;
            _slingshot = slingshot;
        }
        
        public void Enter(HexCell startCell)
        {
            var neighbours = _grid.FindColorCluster(startCell);
            
            switch (neighbours.Count)
            {
                case > 2:
                    _slingshot.StartCoroutine(PopBubbles(neighbours));
                    _stateMachine.ChangeState<CheckFloatingBubblesState>();
                    break;
                default:
                    _stateMachine.ChangeState<ReloadSlingshotState>();
                    break;
            }
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
                Debug.Log($"лопаю {cell.OffsetCoordinates.Column} | {cell.OffsetCoordinates.Row}");
                cell.PopBubble();
                yield return _wait;
            }
        }
    }
}