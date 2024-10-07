using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class LevelCompleteState : IState
    {
        private readonly StateMachine _stateMachine;
        //UI
        
        public LevelCompleteState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Enter()
        {
           //UI show level complete
           _stateMachine.ChangeState<LoadLevelState>();
        }

        public void Exit()
        {
        }
    }
}