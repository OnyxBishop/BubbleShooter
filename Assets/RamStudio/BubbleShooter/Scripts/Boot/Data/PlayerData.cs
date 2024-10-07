using System;
using RamStudio.BubbleShooter.Scripts.Boot.Interfaces;
using RamStudio.BubbleShooter.Scripts.Common.Enums;

namespace RamStudio.BubbleShooter.Scripts.Boot.Data
{
    [Serializable]
    public class PlayerData : ISavable
    {
        public string SaveName => SaveNames.PlayerData.ToString();
    }
}