using System;

namespace RamStudio.BubbleShooter.Scripts.GUI.Interfaces
{
    public interface IValueView<in T>
    where T : IComparable
    {
        public void UpdateText(T amount);
    }
}