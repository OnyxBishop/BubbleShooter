using System;
using RamStudio.BubbleShooter.Scripts.GUI;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public class StoragePresenter : IDisposable
    {
        private readonly ScoreStorage _scoreStorage;
        private readonly IntValueView _intValueView;
        
        public StoragePresenter(ScoreStorage scoreStorage, IntValueView intValueView)
        {
            _scoreStorage = scoreStorage;
            _intValueView = intValueView;

            OnValueChanged(_scoreStorage.Points);
            _scoreStorage.Changed += OnValueChanged;
        }

        public void Dispose() => 
            _scoreStorage.Changed -= OnValueChanged;

        private void OnValueChanged(int amount) 
            => _intValueView.UpdateText(amount);
    }
}