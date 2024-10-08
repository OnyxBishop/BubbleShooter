using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI.Popups
{
    public class GameEndPopup : Popup
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _toBenuButton;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private RectTransform _starsImagesContainer;

        public event Action RestartClicked;
        public event Action GoToMenuClicked;

        public void Init(string finalScore, bool isWin)
        {
            _finalScoreText.text = finalScore;

            for (var i = 0; i < _starsImagesContainer.childCount; i++)
                _starsImagesContainer.GetChild(i).gameObject.SetActive(isWin);
        }

        private void OnEnable()
        {
            _restartButton?.onClick.AddListener(OnRestartButtonClicked);
            _toBenuButton?.onClick.AddListener(OnMenuButtonClicked);
        }

        private void OnDisable()
        {
            _restartButton?.onClick.RemoveListener(OnRestartButtonClicked);
            _toBenuButton?.onClick.RemoveListener(OnMenuButtonClicked);
        }

        private void OnRestartButtonClicked()
            => RestartClicked?.Invoke();

        private void OnMenuButtonClicked()
            => GoToMenuClicked?.Invoke();
    }
}