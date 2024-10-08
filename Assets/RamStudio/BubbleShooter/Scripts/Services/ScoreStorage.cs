using System;
using RamStudio.BubbleShooter.Scripts.Boot.Data;
using RamStudio.BubbleShooter.Scripts.Common.Enums;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public class ScoreStorage
    {
        private readonly SaveLoadService _saveLoadService;
        private readonly PlayerData _savableData;
        private int _points;

        public ScoreStorage(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _savableData = _saveLoadService.LoadFromPrefs(SaveNames.PlayerData);
            _points = 0;
        }

        public event Action<int> Changed;

        public int Points => _points;
        
        public void Add(int amount)
        {
            _points += amount;
            _savableData.ScorePoints = _points;
            
            _saveLoadService.SaveToPrefs(_savableData);
            Changed?.Invoke(_points);
        }
    }
}