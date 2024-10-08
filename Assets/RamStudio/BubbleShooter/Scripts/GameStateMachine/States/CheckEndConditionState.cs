using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.GUI.Popups;
using RamStudio.BubbleShooter.Scripts.Services;
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
        private readonly ScoreStorage _scoreStorage;
        private readonly int _winThreshold;

        public CheckEndConditionState(StateMachine stateMachine, LevelConfiguration configuration,
            BubblesStorage bubblesStorage, AmmoStorage ammoStorage, ScoreStorage scoreStorage)
        {
            _stateMachine = stateMachine;
            _bubblesStorage = bubblesStorage;
            _ammoStorage = ammoStorage;
            _scoreStorage = scoreStorage;
            _winThreshold =
                Mathf.CeilToInt(_bubblesStorage.Count * (configuration.BubblesLeftWinConditionPercent / 100f));
        }

        public void Enter()
        {
            if (_bubblesStorage.Count <= _winThreshold || _bubblesStorage.Count <= 3)
            {
                var prefab = Resources.Load<GameEndPopup>(AssetPaths.GameWinPopup);
                var instance = Object.Instantiate(prefab);
                instance.Init(_scoreStorage.Points.ToString(), isWin: true);
                _stateMachine.ChangeState<GameEndState, GameEndPopup>(instance);
                return;
            }

            if (!_ammoStorage.HasBubbles)
            {
                var prefab = Resources.Load<GameEndPopup>(AssetPaths.GameLosePopup);
                var instance = Object.Instantiate(prefab);
                instance.Init(_scoreStorage.Points.ToString(), isWin: false);
                _stateMachine.ChangeState<GameEndState, GameEndPopup>(instance);
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