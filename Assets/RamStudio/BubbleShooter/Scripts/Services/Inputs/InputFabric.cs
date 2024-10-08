using System;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RamStudio.BubbleShooter.Scripts.Services.Inputs
{
    public class InputFabric
    {
        private readonly Transform _container;
        private readonly MobileInput _mobileInput;
        private readonly MouseInput _mouseInput;

        public InputFabric(Transform container)
        {
            _container = container;
            _mobileInput = Resources.Load<MobileInput>(AssetPaths.MobileInput);
            _mouseInput = Resources.Load<MouseInput>(AssetPaths.MouseInput);
        }

        public IInput Create(DeviceType type)
        {
            return type switch
            {
                DeviceType.Handheld => Object.Instantiate(_mobileInput, _container),
                DeviceType.Desktop => Object.Instantiate(_mouseInput, _container),
                _ => throw new ArgumentNullException()
            };
        }
    }
}