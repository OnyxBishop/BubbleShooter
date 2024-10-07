using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class PlayerInputState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly Slingshot _slingshot;
        private readonly InputSystem _inputSystem;
        
        public PlayerInputState(StateMachine stateMachine, Slingshot slingshot, InputSystem inputSystem)
        {
            _stateMachine = stateMachine;
            _slingshot = slingshot;
            _inputSystem = inputSystem;
        }
        
        public void Enter()
        {
            _inputSystem.Enable();
            _slingshot.Shot += OnShoot;
        }

        public void Exit()
        {
            _inputSystem.Disable();
            _slingshot.Shot -= OnShoot;
        }

        private void OnShoot(Bubble bubble)
        {
            _stateMachine.ChangeState<BallFlightState, Bubble>(bubble);
        }
    }
}