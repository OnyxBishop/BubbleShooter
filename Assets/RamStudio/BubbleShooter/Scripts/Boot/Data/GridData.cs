using System;
using RamStudio.BubbleShooter.Scripts.Boot.Interfaces;
using RamStudio.BubbleShooter.Scripts.Common.Enums;

namespace RamStudio.BubbleShooter.Scripts.Boot.Data
{
    [Serializable]
    public class GridData : ISavable
    {
        public int Columns;
        public int Rows;
        public BubbleColors[] BubblesArray;
        public string Id;
        
        public string SaveName => $"{SaveNames.Grid}{Id}";
    }
}