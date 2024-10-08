using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class PlayerInputState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly Slingshot _slingshot;
        private readonly InputService _inputService;
        
        public PlayerInputState(StateMachine stateMachine, Slingshot slingshot, InputService inputService)
        {
            _stateMachine = stateMachine;
            _slingshot = slingshot;
            _inputService = inputService;
        }
        
        public void Enter()
        {
            _inputService.Enable();
            _slingshot.Shot += OnShoot;
        }

        public void Exit()
        {
            _inputService.Disable();
            _slingshot.Shot -= OnShoot;
        }

        private void OnShoot(Bubble bubble)
        {
            _stateMachine.ChangeState<BallFlightState, Bubble>(bubble);
        }
    }
}