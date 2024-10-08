using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Grid;
using RamStudio.BubbleShooter.Scripts.GUI;
using RamStudio.BubbleShooter.Scripts.GUI.Popups;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class GameEndState : IStateWithPayload<GameEndPopup>
    {
        private readonly StateMachine _stateMachine;
        private readonly GameplayHUD _gameplayHUD;
        private readonly StoragePresenter _scorePresenter;
        private readonly AmmoStoragePresenter _ammoPresenter;
        private readonly HexGrid _grid;

        private GameEndPopup _endPopup;

        public GameEndState(StateMachine stateMachine, GameplayHUD gameplayHUD,
            StoragePresenter presenter, AmmoStoragePresenter ammoStoragePresenter, HexGrid grid)
        {
            _stateMachine = stateMachine;
            _gameplayHUD = gameplayHUD;
            _scorePresenter = presenter;
            _ammoPresenter = ammoStoragePresenter;
            _grid = grid;
        }

        public void Enter(GameEndPopup popup)
        {
            _endPopup = popup;
            _grid.DropAllBubbles(this, OnAllBubblesDown);
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            _scorePresenter.Dispose();
            _ammoPresenter.Dispose();
        }

        private void OnAllBubblesDown()
        {
            _endPopup.RectTransform.SetParent(_gameplayHUD.Content, false);
            _endPopup.RectTransform.anchoredPosition = _gameplayHUD.Content.pivot;

            _endPopup.Open();
            _endPopup.RestartClicked += OnRestartClicked;
            _endPopup.GoToMenuClicked += OnMenuButtonClicked;
        }

        private void OnRestartClicked()
        {
            _endPopup.RestartClicked -= OnRestartClicked;
            _stateMachine.ChangeState<SwitchSceneState, SceneNames>(SceneNames.Gameplay);
        }

        private void OnMenuButtonClicked()
        {
            _endPopup.GoToMenuClicked -= OnMenuButtonClicked;
            _stateMachine.ChangeState<SwitchSceneState, SceneNames>(SceneNames.Menu);
        }
    }
}