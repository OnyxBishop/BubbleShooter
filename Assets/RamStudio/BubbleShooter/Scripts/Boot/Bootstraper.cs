using System.Collections;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Boot
{
    public class Bootstraper : MonoBehaviour
    {
        private IEnumerator Start()
        {
            //add saveSys.Init();
            //add PauseService.Init();

            yield return GetDeviceType();
            yield return SceneSwitcher.ChangeToAsync(SceneNames.Menu);
        }

        private IEnumerator GetDeviceType()
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
                BootstrapInfo.InitByDeviceType(DeviceType.Handheld);
            else if (SystemInfo.deviceType == DeviceType.Desktop)
                BootstrapInfo.InitByDeviceType(DeviceType.Desktop);
            
            yield break;
        }
    }
}