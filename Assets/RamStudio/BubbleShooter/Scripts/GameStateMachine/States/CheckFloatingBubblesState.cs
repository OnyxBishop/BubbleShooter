using DG.Tweening;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class CheckFloatingBubblesState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly HexGrid _grid;
        private readonly float _fallDuration = 1.5f;
        private readonly float _fallOffset = 0.2f;

        public CheckFloatingBubblesState(StateMachine stateMachine, HexGrid grid)
        {
            _stateMachine = stateMachine;
            _grid = grid;
        }

        public void Enter()
        {
            var floatingCluster = _grid.FindFloatingCluster();

            if (floatingCluster.Count > 0)
                foreach (var cell in floatingCluster)
                    AnimateBubbleFall(cell);

            _stateMachine.ChangeState<ReloadSlingshotState>();
        }

        public void Exit()
        {
        }

        private void AnimateBubbleFall(HexCell cell)
        {
            var bubble = cell.Bubble;

            bubble.transform.DOMoveY(_grid.Bounds.Bottom.y - _fallOffset, _fallDuration).OnComplete(cell.PopBubble);
        }
    }
}