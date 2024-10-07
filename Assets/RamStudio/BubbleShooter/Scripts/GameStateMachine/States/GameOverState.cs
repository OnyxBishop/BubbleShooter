using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class GameOverState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly RectTransform _container;
        
        public GameOverState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Enter()
        {
            var prefab = Resources.Load<GameLosePopup>(AssetPaths.GameLosePopup);
            var instance = Object.Instantiate(prefab, _container);
        }
        
        public void Exit()
        {
        }
    }
}