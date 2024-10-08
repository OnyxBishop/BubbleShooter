using System;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services.Interfaces
{
    public interface IInput
    {
        public event Action<Vector2> BeginDrag;
        public event Action<Vector2> Dragging;
        public event Action EndDrag;

        public void Enable();
        public void Disable();
    }
}