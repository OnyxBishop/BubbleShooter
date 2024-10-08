using System;
using DG.Tweening;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GUI.Popups
{
    public abstract class Popup : MonoBehaviour
    {
        private readonly Vector3 _closeScale = new Vector3(0.1f, 0.1f, 0.1f);
        private readonly Vector3 _normalScale = Vector3.one;
        private readonly float _animationTime = 1f;

        [SerializeField] private RectTransform _container;

        public event Action CloseButtonClicked;
        public RectTransform RectTransform => (RectTransform)gameObject.transform;

        private void Awake()
            => _container.localScale = _closeScale;

        public void Open()
        {
            _container.localScale = _closeScale;
            gameObject.SetActive(true);

            _container
                .DOScale(_normalScale, _animationTime)
                .SetUpdate(true);
        }

        protected void Close()
        {
            _container.transform
                .DOScale(_closeScale, _animationTime)
                .SetUpdate(true)
                .OnComplete(() => gameObject.SetActive(false));

            CloseButtonClicked?.Invoke();
        }
    }
}