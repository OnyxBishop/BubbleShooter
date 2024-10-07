using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class ReloadSlingshotState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly Slingshot _slingshot;
        private readonly SlingshotConnector _connector;
        private readonly BubbleSpawner _spawner;
        private readonly int _shotCount;

        private int _slingshotGivenBalls;

        public ReloadSlingshotState(StateMachine stateMachine, Slingshot slingshot, SlingshotConnector connector,
            BubbleSpawner spawner, int shotCount)
        {
            _stateMachine = stateMachine;
            _slingshot = slingshot;
            _spawner = spawner;
            _connector = connector;
            _shotCount = shotCount;
        }

        public void Enter()
        {
            if (_slingshotGivenBalls <= _shotCount)
            {
                _slingshot.Reload(_spawner.GetLaunchBall());
                _slingshotGivenBalls++;
                
                _stateMachine.ChangeState<PlayerInputState>();
            }
            else
            {
                _connector.Dispose();
                _stateMachine.ChangeState<GameOverState>();
            }
        }

        public void Exit()
        {
        }
    }
}