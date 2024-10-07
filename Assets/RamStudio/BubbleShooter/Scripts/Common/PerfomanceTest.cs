using UnityEngine;
using Debug = UnityEngine.Debug;
using Screen = UnityEngine.Device.Screen;

namespace RamStudio.BubbleShooter.Scripts.Common
{
    public class PerfomanceTest : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);
        }
    }
}