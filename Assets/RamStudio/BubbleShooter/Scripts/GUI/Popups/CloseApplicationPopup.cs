using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI.Popups
{
    public class CloseApplicationPopup : Popup
    {
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _declineButton;

        private void OnEnable()
        {
            _acceptButton.onClick.AddListener(Application.Quit);
            _declineButton.onClick.AddListener(OnDeclineCLicked);
        }

        private void OnDisable()
        {
            _declineButton.onClick.RemoveListener(OnDeclineCLicked);
        }

        private void OnDeclineCLicked()
        {
            Close();
        }
    }
}