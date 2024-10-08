using System;
using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    [RequireComponent(typeof(Button))]
    public class PauseButton : MonoBehaviour
    {
        private Button _button;

        public event Action Clicked;

        private void OnEnable()
        {
            _button ??= GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked() 
            => Clicked?.Invoke();
    }
}