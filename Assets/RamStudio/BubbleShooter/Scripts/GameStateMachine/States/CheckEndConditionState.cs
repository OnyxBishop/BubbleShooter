using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.GUI;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using RamStudio.BubbleShooter.Scripts.SO;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class CheckEndConditionState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly BubblesStorage _bubblesStorage;
        private readonly AmmoStorage _ammoStorage;
        private readonly int _winThreshold;

        public CheckEndConditionState(StateMachine stateMachine, LevelConfiguration configuration,
            BubblesStorage bubblesStorage, AmmoStorage ammoStorage)
        {
            _stateMachine = stateMachine;
            _bubblesStorage = bubblesStorage;
            _ammoStorage = ammoStorage;
            _winThreshold =
                Mathf.CeilToInt(_bubblesStorage.Count * (configuration.BubblesLeftWinConditionPercent / 100f));
        }

        public void Enter()
        {
            if (_bubblesStorage.Count <= _winThreshold)
            {
                var prefab = Resources.Load<GameEndPopup>(AssetPaths.GameWinPopup);
                _stateMachine.ChangeState<GameEndState, GameEndPopup>(prefab);
                return;
            }

            if (!_ammoStorage.HasBubbles)
            {
                var prefab = Resources.Load<GameEndPopup>(AssetPaths.GameLosePopup);
                _stateMachine.ChangeState<GameEndState, GameEndPopup>(prefab);
                return;
            }

            _stateMachine.ChangeState<ReloadSlingshotState>();
        }

        public void Exit()
        {
            _bubblesStorage.Dispose();
        }
    }
}