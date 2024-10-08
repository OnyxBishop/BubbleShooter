using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI.Popups
{
    public class PausePopup : Popup
    {
        [SerializeField] private Button _backToMenuButton;
        [SerializeField] private Button _exitButton;

        private void OnEnable()
        {
            _backToMenuButton.onClick.AddListener(OnBackToMenuButton);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDisable()
        {
            _backToMenuButton.onClick.RemoveListener(OnBackToMenuButton);
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnExitClicked()
        {
            Close();
        }

        private void OnBackToMenuButton()
            => SceneSwitcher.ChangeToAsync(SceneNames.Menu);
    }
}