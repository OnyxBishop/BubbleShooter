using System;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services.Inputs
{
    public class MouseInput : MonoBehaviour, IInput
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
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                BeginDrag?.Invoke(mousePosition);
                _isDragging = true;
            }

            if (Input.GetMouseButton(0) && _isDragging)
            {
                var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                Dragging?.Invoke(mousePosition);
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                EndDrag?.Invoke();
                _isDragging = false;
            }
        }

        public void Enable()
            => enabled = true;

        public void Disable()
            => enabled = false;
    }
}