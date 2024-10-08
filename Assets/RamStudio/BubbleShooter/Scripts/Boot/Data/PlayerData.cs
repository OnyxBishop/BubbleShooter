using System;
using RamStudio.BubbleShooter.Scripts.Boot.Interfaces;
using RamStudio.BubbleShooter.Scripts.Common.Enums;

namespace RamStudio.BubbleShooter.Scripts.Boot.Data
{
    [Serializable]
    public class PlayerData : ISavable
    {
        public int ScorePoints;
        public string SaveName => SaveNames.PlayerData.ToString();
    }
}