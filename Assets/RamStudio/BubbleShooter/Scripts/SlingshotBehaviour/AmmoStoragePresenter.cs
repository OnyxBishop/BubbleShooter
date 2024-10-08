using System;
using System.Linq;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GUI;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SlingshotBehaviour
{
    public class AmmoStoragePresenter : IDisposable
    {
        private readonly AmmoStorage _ammoStorage;
        private readonly AmmoView _ammoView;
        private readonly IntValueView _countView;
        private readonly Sprite[] _sprites;
        
        public AmmoStoragePresenter(AmmoStorage ammoStorage, AmmoView ammoView, Sprite[] sprites)
        {
            _ammoStorage = ammoStorage;
            _ammoView = ammoView;
            _countView = _ammoView.CountView;
            _sprites = sprites;

            _ammoStorage.Changed += OnChanged;
            OnChanged(_ammoStorage.Count, _ammoStorage.CurrentColor);
        }

        public void Dispose()
        {
            _ammoStorage.Changed -= OnChanged;
        }

        private void OnChanged(int remainder, BubbleColors color)
        {
            _ammoView.SetSprite(GetSprite(color));
            _countView.UpdateText(remainder);
        }

        private Sprite GetSprite(BubbleColors color)
        {
            var sprite = _sprites.FirstOrDefault(sprite => sprite.name == color.ToString());

            if (!sprite)
                throw new Exception($"Cannot found sprite for color {color.ToString()}");

            return sprite;
        }
    }
}