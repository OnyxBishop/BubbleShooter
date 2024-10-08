using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class ReloadSlingshotState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly Slingshot _slingshot;
        private readonly SlingshotConnector _connector;
        private readonly AmmoStorage _ammoStorage;

        public ReloadSlingshotState(StateMachine stateMachine, Slingshot slingshot, SlingshotConnector connector,
            AmmoStorage ammoStorage)
        {
            _stateMachine = stateMachine;
            _slingshot = slingshot;
            _connector = connector;
            _ammoStorage = ammoStorage;
        }

        public void Enter()
        {
            if (_ammoStorage.TryGet(out var bubble))
            {
                _slingshot.Reload(bubble);
                _stateMachine.ChangeState<PlayerInputState>();
            }
            else
            {
                _connector.Dispose();
                _stateMachine.ChangeState<CheckEndConditionState>();
            }
        }

        public void Exit()
        {
        }
    }
}