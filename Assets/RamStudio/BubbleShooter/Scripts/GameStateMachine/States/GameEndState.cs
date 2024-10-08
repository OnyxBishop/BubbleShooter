using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.GUI;
using RamStudio.BubbleShooter.Scripts.Services;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class GameEndState : IStateWithPayload<GameEndPopup>
    {
        private readonly StateMachine _stateMachine;
        private readonly GameplayHUD _gameplayHUD;
        private readonly ScoreStorage _scoreStorage;
        private readonly StoragePresenter _presenter;

        private GameEndPopup _endPopup;

        public GameEndState(StateMachine stateMachine, GameplayHUD gameplayHUD, ScoreStorage scoreStorage,
            StoragePresenter presenter)
        {
            _stateMachine = stateMachine;
            _gameplayHUD = gameplayHUD;
            _scoreStorage = scoreStorage;
            _presenter = presenter;
        }

        public void Enter(GameEndPopup popup)
        {
            _endPopup = Object.Instantiate(popup);
            _endPopup.RectTransform.anchoredPosition = _gameplayHUD.Content.pivot;

            _endPopup.Init(_scoreStorage.Points.ToString());
            _endPopup.AcceptClicked += AcceptClicked;
            _endPopup.DeclineClicked += OnDeclineClicked;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            _presenter.Dispose();
        }

        private void AcceptClicked()
        {
            _endPopup.AcceptClicked -= AcceptClicked;
            _stateMachine.ChangeState<LoadLevelState>();
        }

        private void OnDeclineClicked()
        {
            _endPopup.DeclineClicked -= OnDeclineClicked;
            _stateMachine.ChangeState<SwitchSceneState, SceneNames>(SceneNames.Menu);
        }
    }
}