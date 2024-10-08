using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GUI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class MenuButtons : MonoBehaviour
    {
        [SerializeField] private Button _startGame;
        [SerializeField] private Button _aboutGame;
        [SerializeField] private Button _closeGame;
        [SerializeField] private CloseApplicationPopup _closeApplication;
        
        private void OnEnable()
        {
            _startGame.onClick.AddListener(OnStartGameClicked);
            _aboutGame.onClick.AddListener(OnAboutClicked);
            _closeGame.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            _startGame.onClick.RemoveListener(OnStartGameClicked);
            _aboutGame.onClick.RemoveListener(OnAboutClicked);
            _closeGame.onClick.RemoveListener(OnCloseClicked);
        }

        private void OnStartGameClicked()
        {
            SceneSwitcher.ChangeToAsync(SceneNames.Gameplay);
        }

        private void OnAboutClicked()
        {
            SceneSwitcher.ChangeToAsync(SceneNames.ProjectInfo);
        }

        private void OnCloseClicked()
        {
            _closeApplication.Open();
        }
    }
}