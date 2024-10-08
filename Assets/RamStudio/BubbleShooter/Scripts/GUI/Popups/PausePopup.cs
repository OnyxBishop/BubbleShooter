using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class PausePopup : Popup
    {
        [SerializeField] private Button _backToMenuButton;

        private void OnEnable() 
            => _backToMenuButton.onClick.AddListener(OnButtonClicked);

        private void OnDisable() 
            => _backToMenuButton.onClick.RemoveListener(OnButtonClicked);

        private void OnButtonClicked() 
            => SceneSwitcher.ChangeToAsync(SceneNames.Menu);
    }
}