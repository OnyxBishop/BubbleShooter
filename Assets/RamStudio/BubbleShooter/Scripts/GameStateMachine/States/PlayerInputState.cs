using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class PlayerInputState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly Slingshot _slingshot;
        private readonly IInput _input;
        
        public PlayerInputState(StateMachine stateMachine, Slingshot slingshot, IInput input)
        {
            _stateMachine = stateMachine;
            _slingshot = slingshot;
            _input = input;
        }
        
        public void Enter()
        {
            _input.Enable();
            _slingshot.Shot += OnShoot;
        }

        public void Exit()
        {
            _input.Disable();
            _slingshot.Shot -= OnShoot;
        }

        private void OnShoot(Bubble bubble)
        {
            _stateMachine.ChangeState<BallFlightState, Bubble>(bubble);
        }
    }
}