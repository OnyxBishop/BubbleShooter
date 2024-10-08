using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Common
{
    public static class BootstrapInfo
    {
        public static DeviceType DeviceType { get; private set; }
        public static float SafeAreaOffsetY { get; private set; }

        public static void InitByDeviceType(DeviceType type)
        {
            DeviceType = type;
            
            SafeAreaOffsetY = 1.5f;
        }
    }
}