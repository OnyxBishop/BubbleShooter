using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GUI.Popups;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _footer;
        [SerializeField] private PauseButton _pauseButton;
        
        private IInput _input;
        private Popup _pausePopup;

        public RectTransform Content => _container;
        public RectTransform Footer => _footer;
        
        private void OnEnable()
        {
            _pauseButton.Clicked += OnPauseClicked;
        }

        private void OnDisable()
        {
            _pauseButton.Clicked -= OnPauseClicked;
        }

        public void Init(IInput input)
        {
            _input = input;
        }
        
        private void OnPauseClicked()
        {
            if (!_pausePopup)
            {
                var prefab = Resources.Load<Popup>(AssetPaths.PausePopup);
                _pausePopup = Instantiate(prefab, _container, false);
            }
        
            _input.Disable();
            PauseService.Pause();
        
            _pausePopup.Open();
            _pausePopup.CloseButtonClicked += OnClosePauseClicked;
        }

        private void OnClosePauseClicked()
        {
            _input.Enable();
            PauseService.Resume();
        }
    }
}