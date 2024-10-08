using System;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services.Inputs
{
    public class MobileInput : MonoBehaviour, IInput
    {
        public event Action<Vector2> BeginDrag;
        public event Action<Vector2> Dragging;
        public event Action EndDrag;

        private Camera _camera;
        private bool _isDragging;

        private void Awake()
            => _camera = Camera.main;

        private void Update()
        {
            if (Input.touchCount <= 0)
                return;
            
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    var beginPosition = _camera.ScreenToWorldPoint(touch.position);
                    BeginDrag?.Invoke(beginPosition);
                    _isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (_isDragging)
                    {
                        var dragPosition = _camera.ScreenToWorldPoint(touch.position);
                        Dragging?.Invoke(dragPosition);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_isDragging)
                    {
                        EndDrag?.Invoke();
                        _isDragging = false;
                    }
                    break;
            }
        }
        
        public void Enable()
            => enabled = true;

        public void Disable()
            => enabled = false;
    }
}