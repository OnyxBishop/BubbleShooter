using System;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public class StoragePresenter : IDisposable
    {
        private readonly ScoreStorage _scoreStorage;
        private readonly ValueView _valueView;
        
        public StoragePresenter(ScoreStorage scoreStorage, ValueView valueView)
        {
            _scoreStorage = scoreStorage;
            _valueView = valueView;

            OnValueChanged(_scoreStorage.Points);
            _scoreStorage.Changed += OnValueChanged;
        }

        public void Dispose() => 
            _scoreStorage.Changed -= OnValueChanged;

        private void OnValueChanged(int amount) 
            => _valueView.UpdateText(amount);
    }
}